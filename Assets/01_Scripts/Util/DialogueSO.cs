using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DialogueSO : ScriptableObject
{
	public Dictionary<string, DialogueData> Dialogue = new();
}
