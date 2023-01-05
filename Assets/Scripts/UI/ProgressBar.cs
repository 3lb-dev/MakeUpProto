using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressFill;

    private void OnEnable()
    {
        MakeUpManager.OnMaskUpdate += UpdateProgressBar;
    }

    private void OnDisable()
    {
        MakeUpManager.OnMaskUpdate -= UpdateProgressBar;
    }

    void UpdateProgressBar(int totalElements, int currentElements)
    {
        float progress = (float)currentElements / totalElements;
        progressFill.fillAmount = progress;
    }
}
