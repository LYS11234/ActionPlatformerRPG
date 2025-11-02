using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterController
{
    #region Player Components
    [SerializeField]
    protected CameraManager playerCam;
    #endregion
    [SerializeField]
    private int bulletCount;
    public event Action<int> OnShotFired;
    private Vector2 gunDirection = Vector2.zero;
    

    private RaycastHit2D[] rayHits = new RaycastHit2D[byte.MaxValue];
    private Collider2D[] overlapHits = new Collider2D[byte.MaxValue];
   
    private List<CharacterController> hitList = new List<CharacterController>();


    protected override void Start()
    {
        base.Start();
        StandOffset = new Vector2(collider.offset.x, collider.offset.y);
        StandSize = new Vector2(collider.size.x, collider.size.y);
        CrouchOffset = new Vector2(collider.offset.x, -0.5f);
        CrouchSize = new Vector2(collider.size.x, StandSize.y * 0.5f);
        StartUI();
    }

    public void StartUI()
    {
        OnShotFired?.Invoke(bulletCount);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();        
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
        if(!canAttack)
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


    #region Attack

    public override void Attack()
    {
        base.Attack();
        NextAttackMotion();
        Source.clip = AttackSounds[(int)AttackMotion - 1];
        Source.Play();
        // 공격 함수 추가
        AttackCheck();
        Animator.SetFloat(attackHash, AttackMotion);
        Animator.SetTrigger(isAttackHash);
    }

    protected override void AttackCheck()
    {
        base.AttackCheck();
        int num = (int)AttackMotion - 1;
        Vector2 attackPos = new Vector2(characterTransform.position.x + attackPoses[num].x * dir, characterTransform.position.y + attackPoses[num].y);

        int _count = Physics2D.OverlapBox(attackPos, attackSizes[num], 0, contactFilter, overlapHits);

        for (int i = 0; i < _count; i++)
        {
            if (!overlapHits[i].TryGetComponent<CharacterController>(out CharacterController hitChar))
            {
                continue;
            }
            hitList.Add(hitChar);
        }

        if (hitList.Count <= 0)
        {
            return;
        }
        switch (AttackMotion)
        {
            case 3:
                {
                    ThirdAttack(hitList);
                    break;
                }
            default:
                {
                    hitList[0].GetDamage(1);
                    break;
                }
        }
        hitList.Clear();
        
    }

    private void ThirdAttack(List<CharacterController> hits)
    {
        for(int i = 0; i < hits.Count; i++)
        {
            hits[i].GetDamage(1);
        }
    }

    public override void CheckBulletHit()
    {
        base.CheckBulletHit();
        gunDirection.x = dir;
        if (MoveInput.y < 0)
        {
            gunDirection.y = -1;
        }
        else if (MoveInput.y > 0)
        {
            gunDirection.y = 1;
        }
        else
        {
            gunDirection.y = 0;
        }
        int _count = Physics2D.Raycast(characterTransform.position, gunDirection, contactFilter, rayHits, 100);

        for (int i = 0; i < _count; i++)
        {
            if (!rayHits[i].transform.TryGetComponent<CharacterController>(out CharacterController hitChar))
            {
                continue;
            }
            hitList.Add(hitChar);
        }
        if (hitList.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < hitList.Count; i++)
        {
            hitList[i].GetDamage(2);
        }
        hitList.Clear();
    }

    public override void Fire()
    {
        base.Fire();
        ChangeCanAttack();
        Animator.SetFloat(shootHash, MoveInput.y);
        Animator.SetTrigger(isShootHash);
        CheckBulletHit();
    }

    #endregion
    public override void StartJump()
    {
        base.StartJump();
        Animator.SetBool(jumpHash, true);
        int num = UnityEngine.Random.Range(0, JumpSounds.Length);
        Source.clip = JumpSounds[num];
        Source.Play();
    }

    public override void Jump()
    {
        base.Jump();
    }
}
