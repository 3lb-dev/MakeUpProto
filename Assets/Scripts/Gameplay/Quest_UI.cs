using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_UI : MonoBehaviour
{
    public string startQuestID;
    public string endQuestID;
    
    public Sprite completedSprite;
    public Image questImage;
    public RectTransform rectTransform;
    public float activeQuestScaleFactor;
    [SerializeField] private Vector2 activeQuestRectSize;
    [SerializeField] private Vector2 rectSize;
    [SerializeField] private float scaleSpeed;

    private void OnEnable()
    {
        MakeUpManager.OnNewQuest += ActivateQuest;
        MakeUpManager.OnQuestCompleted += SetCompleted;
    }

    private void OnDisable()
    {
        MakeUpManager.OnNewQuest -= ActivateQuest;
        MakeUpManager.OnQuestCompleted -= SetCompleted;
    }

    public void SetupComponent(QUEST_INFO questInfo)
    {
        startQuestID = questInfo.startQuestID;
        endQuestID = questInfo.endQuestID;
        questImage.sprite = questInfo.questSprite;
    }
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        activeQuestRectSize = rectSize * activeQuestScaleFactor;
    }
    void ActivateQuest(string v_questID)
    {
        if (startQuestID == v_questID)
        {
            StartCoroutine(ScaleLerpRect(true));
        }
    }

    void SetCompleted(string v_questID)
    {
        if (endQuestID == v_questID)
        {
            questImage.sprite = completedSprite;
            StartCoroutine(ScaleLerpRect(false));
        }
    }

    IEnumerator ScaleLerpRect(bool scaleUp)
    {
        float v_current = 0;
        float v_target = 1;
        if (!scaleUp)
        {
            v_current = 1;
            v_target = 0;
        }
        while (v_current != v_target)
        {
            v_current = Mathf.MoveTowards(v_current, v_target, Time.deltaTime * scaleSpeed);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(rectSize.x, activeQuestRectSize.x, v_current));
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(rectSize.y, activeQuestRectSize.y, v_current));
            yield return null;
        }
    }
}
