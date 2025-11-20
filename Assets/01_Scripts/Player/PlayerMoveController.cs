using System;
using UnityEngine;
using static Unity.VisualScripting.Member;

[System.Serializable]
public struct MoveElements
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
}

public class PlayerMoveController
{ 
    #region Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;
    private Action<ICharacterState> ChangeState;
    private ICharacterState fallState;
    private ICharacterState idleState;
    #endregion
    #region Hashes
    private int moveHash = Animator.StringToHash("Move");
    private int jumpHash = Animator.StringToHash("Jump");
    private int velYHash = Animator.StringToHash("VelY");
    private int crouchHash = Animator.StringToHash("Crouch");
    #endregion

    #region Jump Stats
    public bool IsJump { get { return animator.GetBool(jumpHash); } }
    #endregion

    private MoveElements moveElements;

    public void Init(SpriteRenderer _spriteRenderer, Animator _animator, Rigidbody2D _rigidBody, BoxCollider2D _collider, Action<ICharacterState> _action, ICharacterState _fallState, ICharacterState _idleState, MoveElements _moveElements)
    {
        spriteRenderer = _spriteRenderer;
        animator = _animator;
        rigidBody = _rigidBody;
        collider = _collider;
        ChangeState = _action;
        fallState = _fallState;
        idleState = _idleState;
        moveElements = _moveElements;
        moveElements.StandOffset = new Vector2(collider.offset.x, collider.offset.y);
        moveElements.StandSize = new Vector2(collider.size.x, collider.size.y);
        moveElements.CrouchOffset = new Vector2(collider.offset.x, -0.5f);
        moveElements.CrouchSize = new Vector2(collider.size.x, moveElements.StandSize.y * 0.5f);
    }

    public void CheckDir(Vector2 _moveInput)
    {
        if (_moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (_moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }


    #region Move
    public void IsRun()
    {
        moveElements.CurrentMoveSpeed = moveElements.MoveSpeed * moveElements.RunSpeed + (moveElements.MoveSpeed * moveElements.AdditionalSpeed);
        animator.SetFloat(moveHash, 2);
    }

    public void IsWalk()
    {
        moveElements.CurrentMoveSpeed = moveElements.MoveSpeed + (moveElements.MoveSpeed * moveElements.AdditionalSpeed);
        animator.SetFloat(moveHash, 1);
    }



    public void ResetMove()
    {
        animator.SetFloat(moveHash, 0);
    }
    public void Move(Vector2 _moveInput)
    {
        rigidBody.linearVelocityX = _moveInput.x * moveElements.CurrentMoveSpeed;
    }
    #endregion
    #region Jump
    public void StartJump()
    {
        rigidBody.AddForceY(moveElements.JumpForce, ForceMode2D.Impulse);
        animator.SetBool(jumpHash, true);
    }

    public void Jump()
    {
        CheckLinearVelY();
        if (rigidBody.linearVelocityY < 0)
        {
            ChangeState(fallState);
        }
        
    }
    public void CheckLinearVelY()
    {
        animator.SetFloat(velYHash, rigidBody.linearVelocityY);
    }

    public void IsLand()
    {
        animator.SetBool(jumpHash, false);
        animator.SetFloat(velYHash, 0);
    }

    public void Fall()
    {
        CheckLinearVelY();
        if(!IsJump)
        {
            ChangeState(idleState);
        }
    }
    #endregion

    #region Crouch
    public void StartCrouch()
    {
        animator.SetBool(crouchHash, true);
        collider.size = moveElements.CrouchSize;
        collider.offset = moveElements.CrouchOffset;
    }

    public void EndCrouch()
    {
        animator.SetBool(crouchHash, false);
        collider.size = moveElements.StandSize;
        collider.offset = moveElements.StandOffset;
    }

    #endregion
}
