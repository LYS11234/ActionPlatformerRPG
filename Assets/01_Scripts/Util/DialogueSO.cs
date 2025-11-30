using System;
using System.Collections.Generic;
using UnityEngine;

enum DialogueName
{
    Misaki = 0,

}


[CreateAssetMenu(fileName = "DialogueSO", menuName = "ScriptableObject/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public List<List<DialogueData>> Dialogues = new List<List<DialogueData>>();
}

[Serializable]
public class DialogueData
{
    public string NameEN;
    public string DialogueEN;
    public string NameKR;
    public string DialogueKR;
    public string NameJP;
    public string DialogueJP;
    public string SpriteL;
    public string SpriteR;
    public string ConditionName;
    public string Condition;
    public string Voice;
    public string Sound;
}

