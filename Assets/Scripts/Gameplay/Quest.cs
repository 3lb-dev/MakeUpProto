using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public string questID;
    public List<Quest_Item> questItems;

    public delegate void HandleQuestComplete(Quest quest);
    public static HandleQuestComplete OnQuestComplete;

    public bool activateAllQuestItems = false;

    public virtual void OnEnable()
    {
        Quest_Item.OnConditionMet += ReviewConditions;
        MakeUpManager.OnNewQuest += ActivateQuest;
    }

    public virtual void OnDisable()
    {
        Quest_Item.OnConditionMet -= ReviewConditions; 
    }

    public virtual void ActivateQuest(string v_questID)
    {
        if (questID == v_questID)
        {
            if (activateAllQuestItems)
            {
                foreach (Quest_Item QI in questItems)
                    QI.ActivateQuestItem();
            }
            else
            {
                questItems[0].ActivateQuestItem();
            }
        }
            
    }

    public virtual void ReviewConditions(Quest_Item questItem)
    {
        if (questItem.questID == questID)
        {
            if (questItems.Contains(questItem))
            {
                questItems.Remove(questItem);
            }
            if (questItems.Count == 0)
            {
                OnQuestComplete?.Invoke(this);
            }
        }
    }
}

[System.Serializable]
public enum TypeOfQuest
{
    TOUCH_USER,
    TOUCH_TOOL
}

[System.Serializable]
public class QUEST_INFO
{
    public string startQuestID;
    public string endQuestID;
    public Sprite questSprite;
}
