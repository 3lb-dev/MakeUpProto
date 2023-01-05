using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using Cinemachine;

public class MakeUpManager : MonoBehaviour
{
    [Header("Debug")]
    public BrushFiller brushFiller;
    public Transform lookAtTarget;
    public AnimationCurve targetLerpCurve;
    public CinemachineVirtualCamera v_cameraControl;

    [Header("UI")]
    public GameObject nextButton;
    public GameObject nextButtonQuest;
    public GameObject stepCompletePanel;
    public GameObject skipStepPanel;
    public RectTransform toolOriginPos;
    public RectTransform toolTargetPos;
    public Transform questsParent;
    public GameObject quest_UI_Prefab;
    public Transform toolHandleHelper;

    static Transform toolHandle;
    static RectTransform screenToolPosition;
    static RectTransform screenToolPositionOrigin;

    private LevelInstance levelInstance;


    static bool drawing;
    //bool nextStepReady = false;
    static bool toolReady;
    public List<MaskBrush> maskBrushes;
    //Transform currentSpriteTransform;
    float newOrthoSize;
    float previousOrthoSize;
    float transitionSpeed;
    Vector3 originPos;
    Vector3 targetPos;
    Vector3 targetOffset;
    //Transform currentTool;
    List<GameObject> currentQuestsUI;

    int totalBrushes;
    int updatedBrushes;
    int stepIndex = 0;
    int partIndex = 0;

    List<SpriteRenderer> spritesToAdd;
    List<SpriteRenderer> spritesToRemove;

    public delegate void HandleMaskUpdate(int v_totalBrushes, int v_updatedBrushes);
    public static HandleMaskUpdate OnMaskUpdate;
    public delegate void HandleNewQuest(string questID);
    public static HandleNewQuest OnNewQuest;
    public delegate void HandleQuestCompleted(string questID);
    public static HandleQuestCompleted OnQuestCompleted;
    public delegate void HandleToolNeeded(string questID, float toolSize);
    public static HandleToolNeeded OnToolNeeded;
    MakeUpPart currentPart;

    private void OnEnable()
    {
        MaskBrush.OnTouchedByUser += ReviewDrawingStatus;
        Quest.OnQuestComplete += ReviewCurrentQuest;
        //MakeUpTool.OnToolActivated += AssignAndTurnOnTool;
    }

    private void OnDisable()
    {
        MaskBrush.OnTouchedByUser -= ReviewDrawingStatus;
        Quest.OnQuestComplete -= ReviewCurrentQuest;
        //MakeUpTool.OnToolActivated -= AssignAndTurnOnTool;
    }

    //Debug, should be activated by UI
    private void Start()
    {
        toolHandle = toolHandleHelper;
        screenToolPositionOrigin = toolOriginPos;
        screenToolPosition = toolTargetPos;
        partIndex = 0;
        SetupNextPart();
    }

    public void SetupNextPart()
    {
        if (currentQuestsUI != null)
        {
            foreach (GameObject GO in currentQuestsUI)
            {
                Destroy(GO);
            }
            currentQuestsUI.Clear();
        }
        else
        {
            currentQuestsUI = new List<GameObject>();
        }
        partIndex++;
        stepIndex = 0;
        levelInstance = Resources.Load("Level1/Part" + partIndex) as LevelInstance;
        if (levelInstance == null)
            return;
        if (levelInstance.setupObjs_Activate != null)
        {
            foreach (string STR in levelInstance.setupObjs_Activate)
            {
                GameObject newObj = GameObject.Find(STR) as GameObject;
                newObj.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        if (levelInstance.setupObjs_Deactivate != null)
        {
            foreach (string STR in levelInstance.setupObjs_Deactivate)
            {
                GameObject newObj = GameObject.Find(STR) as GameObject;
                newObj.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        foreach (QUEST_INFO QI in levelInstance.questsInfo)
        {
            GameObject newQuestUI = Instantiate(quest_UI_Prefab, questsParent) as GameObject;
            newQuestUI.transform.localScale = Vector3.one;
            newQuestUI.GetComponent<Quest_UI>().SetupComponent(QI);
            currentQuestsUI.Add(newQuestUI);
        }
        SetupNextQuest();
    }

    public void CompleteStep()
    {
        Debug.LogWarning("Step " + stepIndex + " has been completed.");
        stepIndex++;
        //drawing = false;
        //nextStepReady = false;
        updatedBrushes = 0;

        //Erase previous brushes
        foreach (MaskBrush MB in maskBrushes)
        {
            Destroy(MB.gameObject);
        }
        maskBrushes.Clear();
        //Switch visibility status and set on or off accordingly
        foreach (SpriteRenderer SR in spritesToAdd)
        {
            SR.maskInteraction = SpriteMaskInteraction.None;
            SR.enabled = true;
        }
        foreach (SpriteRenderer SR in spritesToRemove)
        {
            SR.maskInteraction = SpriteMaskInteraction.None;
            SR.enabled = false;
        }
        OnQuestCompleted?.Invoke(currentPart.questID);
        if (currentPart.isLastQuest)
            stepCompletePanel.SetActive(true);
        else
        {
            if (currentPart.skipToNextQuest)
            {
                SetupNextQuest();
            }
            else if(!currentPart.offerSkipToNextStep)
            {
                nextButton.SetActive(true);
            }
        }
    }

    //Must be activated by UI
    public void SetupNextQuest()
    {
        if (stepIndex < levelInstance.levelParts.Length)
        {
            StartCoroutine(SetupNextQuestRoutine(levelInstance.levelParts[stepIndex]));
            nextButton.SetActive(false);
            nextButtonQuest.SetActive(false);
        }
    }
    IEnumerator SetupNextQuestRoutine(MakeUpPart v_currentPart)
    {
        if (v_currentPart.shouldCameraMove)
        {
            newOrthoSize = v_currentPart.orthoSize;
            previousOrthoSize = v_cameraControl.m_Lens.OrthographicSize;
            targetOffset = v_currentPart.partOffset;
            originPos = lookAtTarget.position;
            targetPos = GameObject.Find(v_currentPart.nameOfTarget).transform.position + targetOffset;
            transitionSpeed = v_currentPart.zoomSpeed;
            yield return StartCoroutine(TransitionToNewTarget());
        }
        else
        {
            //yield return new WaitForSeconds(.2f);
        }
        if (v_currentPart.isMiddleStep)
        {
            if (v_currentPart.offerSkipToNextStep)
            {
                skipStepPanel.SetActive(true);
            }
            else
            {
                nextButton.SetActive(true);
            }
        }
        currentPart = v_currentPart;
        Debug.LogWarning("Setting up quest " + stepIndex);
        spritesToAdd = new List<SpriteRenderer>();
        spritesToRemove = new List<SpriteRenderer>();
        foreach (string STR in v_currentPart.spritesToAdd)
        {
            SpriteRenderer nextSpriteRenderer = GameObject.Find(STR).GetComponent<SpriteRenderer>();
            spritesToAdd.Add(nextSpriteRenderer);
        }

        foreach (string STR in v_currentPart.spritesToRemove)
        {
            SpriteRenderer nextSpriteRenderer = GameObject.Find(STR).GetComponent<SpriteRenderer>();
            spritesToRemove.Add(nextSpriteRenderer);
        }

        switch (v_currentPart.kindOfUpdate)
        {
            case updateKind.AUTO:
                SetupForAuto(spritesToAdd, spritesToRemove);
                break;
            case updateKind.SIMPLE:
                
                break;
            case updateKind.BRUSH:
                SetupForBrush(spritesToAdd, spritesToRemove, v_currentPart);
                break;
            case updateKind.ANIM:

                break;
            case updateKind.BUTTON:
                nextButtonQuest.SetActive(true);
                break;
        }
        if (v_currentPart.needsTool)
        {
            OnToolNeeded?.Invoke(v_currentPart.questID, v_currentPart.toolSize);
            if(v_currentPart.shouldResetToolPosition)
                StartCoroutine(ToolLerpIn());
        }
        //nextStepReady = true;
        yield return new WaitForSeconds(.05f);
        OnNewQuest?.Invoke(v_currentPart.questID);
    }

    public void SkipTillNextMiddleStep()
    {
        //stepIndex++;
        for (int i = stepIndex; i < levelInstance.levelParts.Length; i++)
        {
            //levelInstance.levelParts[i]
            stepIndex = i;
            if (levelInstance.levelParts[i].isMiddleStep || levelInstance.levelParts[i].isLastQuest)
            {
                SetupNextQuest();
                break;
            }
            OnQuestCompleted?.Invoke(levelInstance.levelParts[i].questID);
            foreach (string STR in levelInstance.levelParts[i].spritesToAdd)
            {
                SpriteRenderer nextSpriteRenderer = GameObject.Find(STR).GetComponent<SpriteRenderer>();
                nextSpriteRenderer.enabled = true;
                Debug.Log(STR + " was added");
            }

            foreach (string STR in levelInstance.levelParts[i].spritesToRemove)
            {
                SpriteRenderer nextSpriteRenderer = GameObject.Find(STR).GetComponent<SpriteRenderer>();
                nextSpriteRenderer.enabled = false;
                Debug.Log(STR + " was removed");
            }
        }
    }

    //void AssignAndTurnOnTool(Transform v_transform)
    //{
    //    currentTool = v_transform;
    //}

    public static IEnumerator ToolLerpIn()
    {
        toolReady = false;
        drawing = false;
        Vector3 convertedOriginPos = Camera.main.ScreenToWorldPoint(screenToolPositionOrigin.transform.position);
        Vector3 convertedTargetPos = Camera.main.ScreenToWorldPoint(screenToolPosition.transform.position);
        float _current = 0f;
        float _target = 1f;
        Vector3 originPos = new Vector3(convertedOriginPos.x, convertedOriginPos.y, 0f);
        Vector3 targetPos = new Vector3(convertedTargetPos.x, convertedTargetPos.y, 0f);
        while (_current != _target)
        {
            //Debug.LogWarning(_current);
            _current = Mathf.MoveTowards(_current, _target, Time.deltaTime * 2f);
            toolHandle.position = Vector3.Lerp(originPos, targetPos, _current);
            yield return null;
        }
        toolReady = true;
    }

    void SetupForBrush(List<SpriteRenderer> v_spritesToAdd, List<SpriteRenderer> v_spritesToRemove, MakeUpPart v_currentPart)
    {
        foreach (SpriteRenderer SR in v_spritesToRemove)
        {
            SR.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            SR.enabled = true;
        }
        foreach (SpriteRenderer SR in v_spritesToAdd)
        {
            SR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            SR.enabled = true;
        }
        if(v_currentPart.focusOnAdd)
            maskBrushes = brushFiller.PopulateSprite(v_spritesToAdd, v_currentPart);
        else
            maskBrushes = brushFiller.PopulateSprite(v_spritesToRemove, v_currentPart);
        totalBrushes = maskBrushes.Count;
        updatedBrushes = 0;
    }

    void SetupForAuto(List<SpriteRenderer> v_spritesToAdd, List<SpriteRenderer> v_spritesToRemove)
    {
        foreach (SpriteRenderer SR in v_spritesToRemove)
        {
            SR.enabled = false;
        }
        foreach (SpriteRenderer SR in v_spritesToAdd)
        {
            SR.enabled = true;
        }
        CompleteStep();
    }

    void ReviewDrawingStatus(MaskBrush v_touchedBrush)
    {
        if (drawing)
        {
            v_touchedBrush.TurnOnBrush();
            updatedBrushes++;
            OnMaskUpdate?.Invoke(totalBrushes, updatedBrushes);
            float percentil = (float)updatedBrushes / totalBrushes;
            if (percentil >= 0.8f)
            {
                //set current quest complete
                CompleteStep();
            }
        }
    }

    IEnumerator TransitionToNewTarget()
    {
        float _current = 0f;
        float _target = 1f;
        while (_current != _target)
        {
            _current = Mathf.MoveTowards(_current, _target, transitionSpeed * Time.deltaTime);
            lookAtTarget.transform.position = Vector3.Lerp(originPos, targetPos, targetLerpCurve.Evaluate(_current));
            v_cameraControl.m_Lens.OrthographicSize = Mathf.Lerp(previousOrthoSize, newOrthoSize, targetLerpCurve.Evaluate(_current));
            yield return null;
        }
    }

    void ReviewCurrentQuest(Quest quest)
    {
        if (quest.questID == currentPart.questID)
        {
            CompleteStep();
        }
    }

    //Tool Management
    private void Update()
    {
        if (toolReady)
        {
            //Debug.LogWarning("Current Tool isn't null");
            if (Application.isMobilePlatform)
            {
                //Debug.LogWarning("Application is mobile");
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    Vector3 pos = touch.position;
                    if (touch.phase == TouchPhase.Began /*&& nextStepReady*/)
                    {
                        SnapToolToTouch(pos);
                        drawing = true;
                    }
                    else if (touch.phase == TouchPhase.Moved && drawing)
                    {
                        SnapToolToTouch(pos);
                    }
                    else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)//End drag or tap
                    {
                        //FingerUp(pos);
                        drawing = false;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) /*&& nextStepReady*/)
                    drawing = true;
                //Debug.LogWarning("Application isn't mobile");
                if (Input.GetMouseButton(0) && drawing)
                {
                    Vector3 pos = Input.mousePosition;
                    SnapToolToTouch(pos);
                }
                if (Input.GetMouseButtonUp(0))
                    drawing = false;
            }
        }
    }

    void SnapToolToTouch(Vector3 inputPosition)
    {
        
            //Debug.LogWarning("Snaping tool to mouse position");
            Vector3 convertedPosition = Camera.main.ScreenToWorldPoint(inputPosition);
            Vector3 resultingPosition = new Vector3(convertedPosition.x, convertedPosition.y, 0f);
            toolHandle.position = resultingPosition;
    }
}