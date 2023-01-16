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
    public bool turnOffOnPlace = false;
    bool toolActive;

    //public delegate void HandleToolActivated(Transform v_transform);
    //public static HandleToolActivated OnToolActivated;

    private void Start()
    {

    }

    private void OnEnable()
    {
        MakeUpManager.OnResetTool += DeactivateTool;
        MakeUpManager.OnToolNeeded += ToolNeeded;
        Quest_Item.OnToolNeeded += ToolNeeded;
        Quest_Item.OnDeactivateTool += DeactivateTool;

    }

    private void OnDisable()
    {
        MakeUpManager.OnResetTool -= DeactivateTool;
        MakeUpManager.OnToolNeeded -= ToolNeeded;
        Quest_Item.OnToolNeeded -= ToolNeeded;
        Quest_Item.OnDeactivateTool -= DeactivateTool;
    }

    void ActivateTool(float newScale)
    {
        collider.enabled = true;
        renderer.enabled = true;
        renderer.transform.localScale = Vector3.one * newScale;
        toolActive = true;
        //OnToolActivated?.Invoke(transform);
    }

    void DeactivateTool(string v_questID)
    {
        if (toolActive)
        {
            foreach (string STR in questList)
            {
                if (STR == v_questID)
                {
                    if (turnOffOnPlace)
                    {
                        collider.enabled = false;
                        renderer.enabled = false;
                    }
                    else
                    {
                        StartCoroutine(LerpOut());
                    }
                    toolActive = false;
                    break;
                }
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

    IEnumerator LerpOut()
    {
        Transform v_parent = transform.parent;
        float v_current = 0f;
        float v_target = 1f;
        Vector3 targetPos;
        if (transform.position.x >= 0)
            targetPos = new Vector3(6, 0, 0);
        else
            targetPos = new Vector3(-6, 0, 0);
        transform.parent = null;
        while (v_current != v_target && renderer.isVisible)
        {
            v_current = Mathf.MoveTowards(v_current, v_target, Time.deltaTime * 0.75f);
            transform.position = Vector3.Lerp(transform.position, targetPos, v_current);
            yield return null;
        }
        collider.enabled = false;
        renderer.enabled = false;
        transform.SetParent(v_parent);
        transform.localPosition = Vector3.zero;
    }

    public Transform GetToolTransform(string v_questID)
    {
        foreach (string STR in questList)
        {
            if (STR == v_questID)
                return collider.transform;
        }
        return null;
    }
}
