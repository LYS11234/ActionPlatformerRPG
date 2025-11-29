using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerController player);
    void Execute(PlayerController player);
    void Exit(PlayerController player);
}


public class PlayerIdleState : IPlayerState
{
    public void Enter(PlayerController player)
    {

    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        if(player.CheckState())
        {
            return;
        }
        //player.MoveView(player.MoveInput);
        if (Mathf.Abs(player.MoveInput.x) > 0.1f)
        {
            player.ChangeState(player.MoveState);
            return;
        }
        if (player.IsShooting)
        {
            player.ChangeState(player.ShootState);
        }
        player.Animator.SetFloat(player.MoveHash, 0);
        //player.ResetCamera();
    }

    public void Exit(PlayerController player)
    {

    }

    // 여기에 코드 작성
}

public class PlayerMoveState : IPlayerState
{
    public void Enter(PlayerController player)
    {

    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        if(player.CheckState())
        {
            return;
        }
        if (Mathf.Abs(player.MoveInput.x) < 0.1f)
        {
            
            player.ChangeState(player.IdleState);
            return;
        }
        IsRun(player);

        player.CheckDir();

        Move(player);
    }
    public void Exit(PlayerController player)
    {
        ResetMove(player);
    }

    private void Move(PlayerController player)
    {
        player.RigidBody.linearVelocityX = player.MoveInput.x * player.MoveElements.CurrentMoveSpeed;
    }

    private void IsRun(PlayerController player)
    {
        if (player.IsSprinting)
        {
            player.MoveElements.Run();
            player.Animator.SetFloat(player.MoveHash, 2);
            return;
        }
        player.MoveElements.Walk();
        player.Animator.SetFloat(player.MoveHash, 1);
    }

    private void ResetMove(PlayerController player)
    {
        player.Animator.SetFloat(player.MoveHash, 0);
    }
}


public class PlayerJumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        StartJump(player); // 왜 두 번 호출되지?
    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.CheckDir();
        Jump(player);
    }
    public void Exit(PlayerController player)
    {
        
    }

    private void CheckLinearVelocityY(PlayerController player)
    {
        player.Animator.SetFloat(player.VelYHash, player.RigidBody.linearVelocityY);
    }

    private void StartJump(PlayerController player)
    {
        player.RigidBody.AddForceY(player.MoveElements.JumpForce, ForceMode2D.Impulse);
        player.Animator.SetBool(player.JumpHash, true);
        player.PlaySound(player.jumpSounds[Random.Range(0, player.jumpSounds.Length)]);
    }

    private void Jump(PlayerController player)
    {
        CheckLinearVelocityY(player);
        
        if(player.RigidBody.linearVelocityY < -0.1f)
        {
            player.ChangeState(player.FallState);
        }
    }
}

public class PlayerFallState : IPlayerState
{
    public void Enter (PlayerController player)
    {

    }

    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.CheckDir();
        Fall(player);

    }

    public void Exit(PlayerController player)
    {

    }

    private void CheckLinearVelocityY(PlayerController player)
    {
        player.Animator.SetFloat(player.VelYHash, player.RigidBody.linearVelocityY);
    }

    private void Fall(PlayerController player)
    {
        CheckLinearVelocityY(player);
        if(player.RigidBody.linearVelocityY > -0.1f)
        {
            player.ChangeState(player.IdleState);
        }
    }
}

public class PlayerCrouchState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        StartCrouch(player);
    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.CheckDir();
        if(player.IsCrouching)
        {
            return;
        }
        player.ChangeState(player.IdleState);
    }
    public void Exit(PlayerController player)
    {
        EndCrouch(player);
    }

    private void StartCrouch(PlayerController player)
    {
        player.Animator.SetBool(player.CrouchHash, true);
        player.Collider.size = player.MoveElements.CrouchSize;
        player.Collider.offset = player.MoveElements.CrouchOffset;
    }

    private void EndCrouch(PlayerController player)
    {
        player.Animator.SetBool(player.CrouchHash, false);
        player.Collider.size = player.MoveElements.StandSize;
        player.Collider.offset = player.MoveElements.StandOffset;
    }
}

public class PlayerAttackState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Attack(player);
    }
    public void Execute(PlayerController player)
    {

    }
    public void Exit(PlayerController player)
    {

    }


    private void Attack(PlayerController player)
    {
        NextAttackMotion(player);
        AttackCheck(player);
        player.PlaySound(player.AttackSounds[(int)player.AttackMotion - 1]);
        player.Animator.SetFloat(player.AttackHash, player.AttackMotion);
        player.Animator.SetTrigger(player.IsAttackHash);
    }

    private void NextAttackMotion(PlayerController player)
    {
        player.CurrentTime = 5;
        if (player.AttackMotion < player.AttackElements.AttackMotionLength)
        {
            player.AttackMotion++;
            return;
        }
        player.AttackMotion = 1;
    }
    private void AttackCheck(PlayerController player)
    {
        int num = (int)player.AttackMotion - 1;
        Vector2 _attackPos = (Vector2)player.PlayerTransform.position + player.AttackElements.AttackHitBoxes[num].Offset * Vector2.right * player.Dir;
        int _count = Physics2D.OverlapBox(_attackPos, player.AttackElements.AttackHitBoxes[num].Size, 0, player.AttackElements.ContactFilter, player.OverlapHits);
        
        for (int i = 0; i < _count; i++)
        {
            if (!player.OverlapHits[i].TryGetComponent(out BoxCollider2D _hit))
            {
                continue;
            }
            player.HitList.Add(_hit);
        }
        if (player.HitList.Count <= 0)
        {
            return;
        }
        switch (player.AttackMotion)
        {
            case 3:
                {
                    ThirdAttack(player, player.HitList);
                    break;
                }
            default:
                {
                    player.Attack?.Invoke(player.HitList[0], player.AttackElements.ATK); // CombatManager에 연결해서 대미지 주는 함수 호출할 것.
                    Debug.Log(player.HitList[0].name);
                    break;
                }
        }
        player.HitList.Clear();
    }

    private void ThirdAttack(PlayerController player, List<BoxCollider2D> _hitList)
    {
        for(int i = 0; i < _hitList.Count; i++)
        {
            player.Attack?.Invoke(_hitList[i], player.AttackElements.ATK);
            Debug.Log(_hitList[i].name);
        }
    }


}
public class PlayerSkillState : IPlayerState
{
    public void Enter(PlayerController player)
    {

    }
    public void Execute(PlayerController player)
    {

    }
    public void Exit(PlayerController player)
    {

    }
}

public class ShootState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Fire(player);
    }
    public void Execute(PlayerController player)
    {

    }
    public void Exit(PlayerController player)
    {

    }

    private void Fire(PlayerController player)
    {
        player.ChangeCanAttack();
        player.Animator.SetFloat(player.ShootHash, player.MoveInput.y);
        player.Animator.SetTrigger(player.IsShootHash);
        CheckBulletHit(player);
    }

    private void CheckBulletHit(PlayerController player)
    {
        player.GunDirection.x = player.Dir;
        player.GunDirection.y = player.MoveInput.y;
        int _count = Physics2D.Raycast(player.PlayerTransform.position, player.GunDirection, player.AttackElements.ContactFilter, player.RayHits);
        for (int i = 0; i < _count; i++)
        {
            if (!player.RayHits[i].transform.TryGetComponent<BoxCollider2D>(out BoxCollider2D hit))
            {
                continue;
            }
            player.HitList.Add(hit);
        }
        if(player.HitList.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < player.HitList.Count; i++)
        {
            player.Attack(player.HitList[i], player.AttackElements.GunATK);
        }
        player.HitList.Clear();
    }
}

public class PlayerDamageState : IPlayerState
{
    public void Enter(PlayerController player)
    {

    }

    public void Execute(PlayerController player)
    {

    }

    public void Exit(PlayerController player)
    {

    }
}