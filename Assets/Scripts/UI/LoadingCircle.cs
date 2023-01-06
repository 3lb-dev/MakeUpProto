using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircle : MonoBehaviour
{
    public RectTransform myTransform;
    public Vector3 offset;
    public float loadingSpeed;
    public string questID;
    public Image loadingFill;
    
    
    Coroutine loadingRoutine;
    Vector3 offScreenPos;

    public delegate void HandleCircleFilled(string v_questID);
    public static HandleCircleFilled OnCircleFilled;

    private void Start()
    {
        offScreenPos = myTransform.localPosition;
    }

    private void OnEnable()
    {
        Quest_Item.OnCollisionStart += FillStart;
        Quest_Item.OnCollisionStop += FillStop;
    }

    private void OnDisable()
    {
        Quest_Item.OnCollisionStart -= FillStart;
        Quest_Item.OnCollisionStop -= FillStop;
    }

    void FillStart(Vector3 screenPos, string v_questID)
    {
        myTransform.position = screenPos + offset;
        questID = v_questID;
        loadingRoutine = StartCoroutine(LoadCircle());
    }

    void FillStop()
    {
        if (loadingRoutine != null)
        {
            StopCoroutine(loadingRoutine);
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        myTransform.localPosition = offScreenPos;
    }

    IEnumerator LoadCircle()
    {
        float v_current = 0f;
        float v_target = 1f;
        while (v_current != v_target)
        {
            v_current = Mathf.MoveTowards(v_current, v_target, Time.deltaTime * 1.5f);
            loadingFill.fillAmount = v_current;
            yield return null;
        }
        ResetPosition();
        Handheld.Vibrate();
        OnCircleFilled?.Invoke(questID);
    }
}
