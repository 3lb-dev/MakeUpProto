using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MaskBrush : MonoBehaviour
{
    public SpriteMask maskBrushRenderer;
    [SerializeField] private TypeOfQuest questType; //debug
    [SerializeField] private int orderInLayerMin; //debug
    [SerializeField] private int orderInLayerMax; //debug

    BoxCollider2D brushCollider;

    public delegate void HandleTouchedByUser(MaskBrush v_brushComponent);
    public static HandleTouchedByUser OnTouchedByUser;
    private void Awake()
    {
        if (brushCollider == null)
            brushCollider = GetComponent<BoxCollider2D>();
    }

    void ReviewCondition(TypeOfQuest typeOfQuest)
    {
        if (typeOfQuest == questType)
        {
            SetConditionMet();
        }
    }

    void SetConditionMet()
    {
        OnTouchedByUser?.Invoke(this);
    }
    private void OnMouseEnter()
    {
        ReviewCondition(TypeOfQuest.TOUCH_USER);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning("Brush was touched by collider");
        MakeUpTool makeUpTool = collision.gameObject.GetComponentInParent<MakeUpTool>();
        if (makeUpTool != null)
        {
            Debug.LogWarning("Brush was touched by tool");
            ReviewCondition(TypeOfQuest.TOUCH_TOOL);
        }
    }

    public void SetupBrush(MakeUpPart v_part)
    {
        questType = v_part.interactionNeeded;
        maskBrushRenderer.transform.localScale = Vector3.one * v_part.brushMaskSize;
        //maskBrushRenderer.frontSortingOrder = v_part.orderInLayerMax;
        //maskBrushRenderer.backSortingOrder = v_part.orderInLayerMin;
        maskBrushRenderer.frontSortingOrder = 200;
        maskBrushRenderer.backSortingOrder = 0;
    }
    public void TurnOnBrush()
    {
        maskBrushRenderer.enabled = true;
        brushCollider.enabled = false;
    }
}
