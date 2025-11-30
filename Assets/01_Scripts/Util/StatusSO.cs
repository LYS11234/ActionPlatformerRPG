using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusSO : ScriptableObject
{
    public Dictionary<string, StatusData> Statuses = new Dictionary<string, StatusData>();
}

[System.Serializable]
public class StatusData
{
    public float OriginATK;
    public float GunATK;
    public float OriginAttackSpeed;
    public float AttackSizeFirstX;
    public float AttackSizeFirstY;
    public float AttackSizeSecondX;
    public float AttackSizeSecondY;
    public float AttackSizeThirdX;
    public float AttackSizeThirdY;
    public float AttackOffsetFirstX;
    public float AttackOffsetFirstY;
    public float AttackOffsetSecondX;
    public float AttackOffsetSecondY;
    public float AttackOffsetThirdX;
    public float AttackOffsetThirdY;
    public bool UseTriggers;
    public bool UseLayerMask;
    public bool UseDepth;
    public bool UseOutsideDepth;
    public string LayerMask;
    public float MinDepth;
    public float MaxDepth;
    public float MinNormalAngle;
    public float MaxNormalAngle;
    public float MoveSpeed;
    public float RunSpeed;
    public float JumpForce;
}