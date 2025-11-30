using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AttackElements
{
    public AttackHitBox[] AttackHitBoxes;   
    public ContactFilter2D ContactFilter;
    public float ATK;
    public float GunATK;
    public float AdditionalATK;
    public float OriginATK;
    public float OriginAttackSpeed;
    public float AdditionalAttackSpeed;
    public float AttackSpeed;
    public int AttackMotionLength { get { return AttackHitBoxes.Length; } }
}

[System.Serializable]
public struct AttackHitBox
{
    public string Name;
    public Vector2 Size;
    public Vector2 Offset;
}

