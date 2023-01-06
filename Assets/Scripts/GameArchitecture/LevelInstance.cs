using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Instance", menuName = "Make Up Game/Level Template")]
public class LevelInstance : ScriptableObject
{
    public string[] setupObjs_Activate;
    public string[] setupObjs_Deactivate;
    public QUEST_INFO[] questsInfo;
    public MakeUpPart[] levelParts;
}

[System.Serializable]
public class MakeUpPart
{
    public string questID;
    [Header("Main Settings")]
    public string[] spritesToAdd;
    public string[] spritesToRemove;
    public updateKind kindOfUpdate;
    public bool focusOnAdd;
    public string guidelineName;
    
    [Header("Control Settings")]
    public bool isLastQuest;
    public bool skipToNextQuest;
    public bool isMiddleStep;
    public bool offerSkipToNextStep;
    public bool needsTool = true;
    public bool shouldResetToolPosition = true;
    public bool keepToolForNextQuest = false;
    public float toolSize = 1f;

    [Header("BRUSH Settings")]
    public Vector2 brushSize;
    [Range(.7f, 1f)]
    public float brushMaskSize = 1.5f;
    public Vector2 pixelCoordinatesInAtlas;
    public Vector2 dimensionsWithinAtlas;
    public bool shouldRemoveLastRow;
    public bool shouldRemoveLastColumn;
    public TypeOfQuest interactionNeeded;

    [Header("SIMPLE Action Settings")]
    public float fadeSpeed;

    [Header("ANIM Action Settings")]
    public float animSpeed;

    [Header("Camera Settings")]
    public bool shouldCameraMove;
    public string nameOfTarget;
    public float orthoSize;
    public Vector3 partOffset;
    public float zoomSpeed;
}

[System.Serializable]
public enum updateKind
{
    BRUSH,
    SIMPLE,
    ANIM,
    AUTO,
    BUTTON
}