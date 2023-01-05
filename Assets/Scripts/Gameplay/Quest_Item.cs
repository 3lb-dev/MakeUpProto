using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Quest_Item : MonoBehaviour
{
    public string questID;
    public bool overrideQuestIdForTool;
    public string subQuestID;

    public TypeOfQuest questType;

    public Quest_Item nextQI;

    public SpriteRenderer[] objectsToTurnOff;
    public SpriteRenderer[] objectsToTurnOn;

    public bool needsTool = true;
    public bool shouldResetToolPos = true;
    public bool shouldTurnOffTool = true;
    public float toolScale = 1f;

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

    private void Start()
    {
        v_collider = GetComponent<PolygonCollider2D>();
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
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
