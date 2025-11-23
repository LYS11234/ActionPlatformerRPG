using System;
using UnityEngine;
using static Unity.VisualScripting.Member;

[System.Serializable]
public class MoveElements
{
    #region Move Stats
    public float CurrentMoveSpeed;
    public float MoveSpeed;
    public float RunSpeed;
    public float AdditionalSpeed;
    #endregion

    #region Jump Stats
    public float JumpForce;
    #endregion

    #region Crouch Stats
    public Vector2 CrouchSize;
    public Vector2 CrouchOffset;
    public Vector2 StandSize;
    public Vector2 StandOffset;
    #endregion

    public void Run()
    {
        CurrentMoveSpeed = MoveSpeed * RunSpeed + (MoveSpeed * AdditionalSpeed);
    }

    public void Walk()
    {
        CurrentMoveSpeed = MoveSpeed + (MoveSpeed * AdditionalSpeed);
    }
}

