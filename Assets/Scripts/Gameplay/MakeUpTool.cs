using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PolygonCollider2D), typeof(SpriteRenderer))]
public class MakeUpTool : MonoBehaviour
{
    public string questID;
    public string[] questList;
    //PolygonCollider2D collider;
    public Collider2D collider;
    public SpriteRenderer renderer;

    //public delegate void HandleToolActivated(Transform v_transform);
    //public static HandleToolActivated OnToolActivated;

    private void Start()
    {

    }

    private void OnEnable()
    {
        MakeUpManager.OnQuestCompleted += DeactivateTool;
        MakeUpManager.OnToolNeeded += ToolNeeded;
        Quest_Item.OnToolNeeded += ToolNeeded;
        Quest_Item.OnDeactivateTool += DeactivateTool;

    }

    private void OnDisable()
    {
        MakeUpManager.OnQuestCompleted -= DeactivateTool;
        MakeUpManager.OnToolNeeded -= ToolNeeded;
        Quest_Item.OnToolNeeded -= ToolNeeded;
        Quest_Item.OnDeactivateTool -= DeactivateTool;
    }

    void ActivateTool(float newScale)
    {
        collider.enabled = true;
        renderer.enabled = true;
        renderer.transform.localScale = Vector3.one * newScale;
        //OnToolActivated?.Invoke(transform);
    }

    void DeactivateTool(string v_questID)
    {
        foreach (string STR in questList)
        {
            if (STR == v_questID)
            {
                collider.enabled = false;
                renderer.enabled = false;
                break;
            }
        }
    }

    void ToolNeeded(string v_questID, float v_toolScale)
    {
        foreach (string STR in questList)
        {
            if (STR == v_questID)
            {
                questID = STR;
                ActivateTool(v_toolScale);
                break;
            }
        }   
    }
}
