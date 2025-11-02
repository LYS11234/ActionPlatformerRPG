using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PlayerController : CharacterController
{
    #region Player Components
    [SerializeField]
    protected CameraManager playerCam;
    #endregion

    public event Action<uint> OnShotFired;

    [SerializeField]
    private uint bulletCount;

   


    protected override void Start()
    {
        base.Start();
        StandOffset = new Vector2(boxCollider.offset.x, boxCollider.offset.y);
        StandSize = new Vector2(boxCollider.size.x, boxCollider.size.y);
        CrouchOffset = new Vector2(boxCollider.offset.x, -0.5f);
        CrouchSize = new Vector2(boxCollider.size.x, StandSize.y * 0.5f);
        
    }

    public void StartUI()
    {
        OnShotFired?.Invoke(bulletCount);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        canAttack = CanAttack;
        
    }

    //public override void MoveView(Vector2 dir)
    //{
    //    base.MoveView(dir);
    //    playerCam.SetView(new Vector3(characterTransform.position.x, characterTransform.position.y + 3 * dir.y, characterTransform.position.z));
    //}

    //public override void ResetCamera()
    //{
    //    base.ResetCamera();
    //    playerCam.SetTarget(characterTransform);
    //}

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        if (MoveInput.x < 0)
        {
            dir = -1;
            return;
        }
        else if (MoveInput.x > 0)
        {
            dir = 1;
            return;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        IsSprinting = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        IsJumping = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        IsAttack = context.ReadValueAsButton();
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        if(bulletCount <= 0)
        {
            IsShooting = false;
            return;
        }
        IsShooting = context.ReadValueAsButton();
    }

    public void ShotBullet()
    {
        bulletCount--;
        OnShotFired.Invoke(bulletCount);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        IsCrouching = context.ReadValueAsButton();
    }

    public void SetIdleState()
    {
        ChangeCanAttack();
        ChangeState(IdleState);
    }

    public override void AttackCheck()
    {
        base.AttackCheck();
        int num = (int)AttackMotion - 1;
        Vector2 attackPos = new Vector2(characterTransform.position.x + attackPoses[num].x * dir, characterTransform.position.y + attackPoses[num].y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPos, attackSizes[num], 0, hitLayer);
        CharacterController[] hitChars = new CharacterController[hits.Length];
        int _count = 0;

        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].TryGetComponent<CharacterController>(out hitChars[_count]))
            {
                continue;
            }
            _count++;
        }

        if (_count <= 0)
        {
            return;
        }
        switch (AttackMotion)
        {
            case 3:
                {
                    ThirdAttack(hitChars);
                    break;
                }
            default:
                {
                    hitChars[0].GetDamage(1);
                    break;
                }
        }

        
    }

    private void ThirdAttack(CharacterController[] hits)
    {
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i] == null)
            {
                break;
            }
            hits[i].GetDamage(1);
        }
    }
}
