using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_UI_Manager : MonoBehaviour
{
    public UI_QuestOptions[] questOptions;
    public RectTransform inScreenAnchor;
    public RectTransform offScreenAnchor;

    private void OnEnable()
    {
        MakeUpManager.OnNewQuest += ActivateQuestOptions;
    }

    private void OnDisable()
    {
        MakeUpManager.OnNewQuest -= ActivateQuestOptions;
    }

    void ActivateQuestOptions(string v_questID)
    {
        foreach (UI_QuestOptions UQO in questOptions)
        {
            if (UQO.questID == v_questID)
            {
                Debug.LogWarning("Found UI element for current quest: " + v_questID);
                foreach (GameObject GO in UQO.questOptions)
                {
                    Debug.LogWarning("Turned on Object: " + GO.name);
                    GO.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject GO in UQO.questOptions)
                {
                    Debug.LogWarning("Turned off Object: " + GO.name);
                    GO.SetActive(false);
                }
            }
        }
    }


}

[System.Serializable]
public struct UI_QuestOptions
{
    public string questID;
    public GameObject[] questOptions;
}
