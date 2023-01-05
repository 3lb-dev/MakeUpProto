using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Accesory : MonoBehaviour
{
    public string questID;
    public string accessoryID;
    public SpriteRenderer v_renderer;

    private void OnEnable()
    {
        Quest_Button.OnButtonPressed += ActivateViaButton;
    }

    private void OnDisable()
    {
        Quest_Button.OnButtonPressed -= ActivateViaButton;
    }
    void ActivateViaButton(string v_questID, string v_accessoryID)
    {
        if (v_questID == questID)
        {
            if (v_accessoryID == accessoryID)
                ActivateAccessory();
            else
                DeactivateAccessory();
        }
    }

    void ActivateAccessory()
    {
        if (v_renderer == null)
            v_renderer = GetComponent<SpriteRenderer>();
        v_renderer.enabled = true;
    }
    void DeactivateAccessory()
    {
        if (v_renderer == null)
            v_renderer = GetComponent<SpriteRenderer>();
        v_renderer.enabled = false;
    }
}
