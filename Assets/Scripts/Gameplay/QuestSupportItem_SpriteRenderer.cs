using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class QuestSupportItem_SpriteRenderer : MonoBehaviour
{
    public string questID;
    public string[] questList;
    public SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        MakeUpManager.OnNewQuest += OnQuestStarted;
        MakeUpManager.OnQuestCompleted += OnQuestEnd;
    }

    private void OnDisable()
    {
        MakeUpManager.OnNewQuest -= OnQuestStarted;
        MakeUpManager.OnQuestCompleted -= OnQuestEnd;
    }

    void OnQuestStarted(string v_questID)
    {
        foreach (string STR in questList)
        {
            if (STR == v_questID)
            {
                ActivateSupportItem();
                questID = STR;
                break;
            }
        }
    }

    void ActivateSupportItem()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
    }

    void OnQuestEnd(string v_questID)
    {
        foreach (string STR in questList)
        {
            if (STR == v_questID)
            {
                DeactivateSupportItem();
                break;
            }
        }
    }

    void DeactivateSupportItem()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    //DEBUG
    [ContextMenu(nameof(SetupComponent))]
    void SetupComponent()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
