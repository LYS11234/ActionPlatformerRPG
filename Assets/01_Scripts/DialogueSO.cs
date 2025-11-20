using System;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSheetSO : ScriptableObject
{
    public List<List<Dialogues>> Dialogues;
}

[Serializable]
public class Dialogues
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

