using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StatusSO : ScriptableObject
{
	public Dictionary<string, StatusData> Status = new();
}
