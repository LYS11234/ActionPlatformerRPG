using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StatusData
{
	public int OriginATK;
	public int GunATK;
	public int OriginAttackSpeed;
	public float AttackSizeFirstX;
	public int AttackSizeFirstY;
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
	public int MinDepth;
	public int MaxDepth;
	public int MinNormalAngle;
	public int MaxNormalAngle;
	public int MoveSpeed;
	public float RunSpeed;
	public int JumpForce;
}
