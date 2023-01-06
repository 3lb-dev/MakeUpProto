using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Quest_Item : MonoBehaviour
{
    [Header("Quest Info")]
    public string questID;
    public bool overrideQuestIdForTool;
    public string subQuestID;
    public TypeOfQuest questType;
    public Quest_Item nextQI;
    public QI_InteractionType interactionType;

    [Header("Quest Objects")]
    public SpriteRenderer[] objectsToTurnOff;
    public SpriteRenderer[] objectsToTurnOn;

    [Header("Tool Info")]
    public bool needsTool = true;
    public bool shouldResetToolPos = true;
    public bool shouldTurnOffTool = true;
    public float toolScale = 1f;

    //[Header("Interactive Quest Info")]


    PolygonCollider2D v_collider;
    bool conditionMet;
    public bool ConditionMet
    {
        get { return conditionMet; }
    }

    public delegate void HandleConditionMet(Quest_Item questItem);
    public static HandleConditionMet OnConditionMet;
    public delegate void HandleToolNeeded(string v_questID, float v_toolScale);
    public static HandleToolNeeded OnToolNeeded;
    public delegate void HandleDeactivateTool(string v_questID);
    public static HandleDeactivateTool OnDeactivateTool;

    public delegate void HandleCollisionStart(Vector3 v_objectScreenPosition, string v_questID);
    public static HandleCollisionStart OnCollisionStart;
    public delegate void HandleCollisionStop();
    public static HandleCollisionStop OnCollisionStop;

    private void Start()
    {
        v_collider = GetComponent<PolygonCollider2D>();
    }

    private void OnEnable()
    {
        LoadingCircle.OnCircleFilled += OnLoadingCircleComplete;
    }

    private void OnDisable()
    {
        LoadingCircle.OnCircleFilled -= OnLoadingCircleComplete;
    }
    public virtual void ActivateQuestItem()
    {
        v_collider.enabled = true;
        if (needsTool)
        {
            if (overrideQuestIdForTool)
                OnToolNeeded?.Invoke(subQuestID, toolScale);
            else
                OnToolNeeded?.Invoke(questID, toolScale);
            if (shouldResetToolPos)
                StartCoroutine(MakeUpManager.ToolLerpIn());
        }
        
    }

    public virtual void SetConditionMet()
    {
        conditionMet = true;
        v_collider.enabled = false;
        if (objectsToTurnOff.Length > 0)
        {
            foreach (SpriteRenderer SR in objectsToTurnOff)
            {
                SR.enabled = false;
            }
        }
        if (objectsToTurnOn.Length > 0)
        {
            foreach (SpriteRenderer SR in objectsToTurnOn)
            {
                SR.enabled = true;
            }
        }
        if (shouldTurnOffTool)
        {
            if (overrideQuestIdForTool)
            {
                OnDeactivateTool?.Invoke(subQuestID);
            }
            else
            {
                OnDeactivateTool?.Invoke(questID);
            }
        }
            
        if (nextQI != null)
            nextQI.ActivateQuestItem();
        OnConditionMet?.Invoke(this);
    }

    void ReviewCondition(TypeOfQuest typeOfQuest)
    {
        if (typeOfQuest == questType)
        {
            SetConditionMet();
        }
    }

    private void OnMouseDown()
    {
        ReviewCondition(TypeOfQuest.TOUCH_USER);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MakeUpTool makeUpTool = collision.gameObject.GetComponentInParent<MakeUpTool>();
        if (makeUpTool != null)
        {
            switch (interactionType)
            {
                case QI_InteractionType.SIMPLE:
                    if (overrideQuestIdForTool)
                    {
                        if (makeUpTool.questID == subQuestID)
                        {
                            ReviewCondition(TypeOfQuest.TOUCH_TOOL);
                        }
                    }
                    else
                    {
                        if (makeUpTool.questID == questID)
                        {
                            ReviewCondition(TypeOfQuest.TOUCH_TOOL);
                        }
                    }
                    break;
                case QI_InteractionType.LOADING:
                    OnCollisionStart(RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position), gameObject.name + questID);
                    break;
            }
            
        }
    }

    void OnLoadingCircleComplete(string v_questID)
    {
        if (gameObject.name + questID == v_questID)
        {
            SetConditionMet();
        }
}

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (interactionType)
        {
            case QI_InteractionType.SIMPLE:
                
                break;
            case QI_InteractionType.LOADING:
                OnCollisionStop?.Invoke();
                break;
        }
    }
}

public enum QI_InteractionType
{
    SIMPLE,
    LOADING,
    ANIM,
    LOADING_ANIM,
    SIMPLE_ANIM
}