using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSupportItem_GameObject : MonoBehaviour
{
    public string[] questIds;
    public GameObject controlledObject;

    private void OnEnable()
    {
        Quest_Button.OnButtonPressed += ActivateGameObject;
    }

    private void OnDisable()
    {
        Quest_Button.OnButtonPressed -= ActivateGameObject;
    }

    void ActivateGameObject(string v_questID, string v_accessoryID)
    {
        foreach (string STR in questIds)
        {
            if (STR == v_questID)
            {
                controlledObject.SetActive(true);
            }
        }
    }
}
