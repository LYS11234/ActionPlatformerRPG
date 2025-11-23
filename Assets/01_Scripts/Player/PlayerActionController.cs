using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AttackElements
{
    public Vector2[] AttackSizes;
    public Vector2[] AttackPoses;
    public int AttackMotionLength;
    public ContactFilter2D ContactFilter;
    public float ATK;
    public float GunATK;
}


