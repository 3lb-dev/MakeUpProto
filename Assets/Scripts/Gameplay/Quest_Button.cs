using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Quest_Button : MonoBehaviour
{
    public string questID;
    public string accessoryID;

    public delegate void HandleButtonPressed(string v_questID, string v_accessoryID);
    public static HandleButtonPressed OnButtonPressed;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ButtonPressed);
    }
    void ButtonPressed()
    {
        OnButtonPressed?.Invoke(questID, accessoryID);
    }
}
