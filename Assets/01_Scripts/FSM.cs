using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        if(player.idleCount >= 6)
        {
            player.idleCount = 0;
            player.ChangeIdleMotion();
        }
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
        player.RigidBody.linearVelocityX = player.MoveInput.x * player.PlayerData.MoveStatus.CurrentMoveSpeed;
    }

    private void IsRun(PlayerController player)
    {
        if (player.IsSprinting)
        {
            player.PlayerData.MoveStatus.Run();
            player.Animator.SetFloat(player.MoveHash, 2);
            return;
        }
        player.PlayerData.MoveStatus.Walk();
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
        StartJump(player);
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
        player.RigidBody.AddForceY(player.PlayerData.MoveStatus.JumpForce, ForceMode2D.Impulse);
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
        if(player.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == player.IdleStateHash && player.RigidBody.linearVelocityY > -0.1f)
        {
            player.ChangeState(player.IdleState);
        }
    }

    private void Fall(PlayerController player)
    {
        CheckLinearVelocityY(player);
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
        Debug.Log("CrouchEnd");
        player.ChangeState(player.IdleState);
    }
    public void Exit(PlayerController player)
    {
        EndCrouch(player);
    }

    private void StartCrouch(PlayerController player)
    {
        player.Animator.SetBool(player.CrouchHash, true);
        player.Collider.size = player.PlayerData.MoveStatus.CrouchSize;
        player.Collider.offset = player.PlayerData.MoveStatus.CrouchOffset;
    }

    private void EndCrouch(PlayerController player)
    {
        player.Animator.SetBool(player.CrouchHash, false);
        player.Collider.size = player.PlayerData.MoveStatus.StandSize;
        player.Collider.offset = player.PlayerData.MoveStatus.StandOffset;
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
        if (player.AttackMotion < player.PlayerData.AttackStatus.AttackMotionLength)
        {
            player.AttackMotion++;
            return;
        }
        player.AttackMotion = 1;
    }
    private void AttackCheck(PlayerController player)
    {
        int num = (int)player.AttackMotion - 1;
        Vector2 _attackPos = (Vector2)player.PlayerTransform.position + player.PlayerData.AttackStatus.AttackHitBoxes[num].Offset * Vector2.right * player.Dir;
        int _count = Physics2D.OverlapBox(_attackPos, player.PlayerData.AttackStatus.AttackHitBoxes[num].Size, 0, player.PlayerData.AttackStatus.ContactFilter, player.OverlapHits);
        
        if(_count <= 0)
        {
            return;
        }


        for (int i = 0; i < _count; i++)
        {
            player.OverlapHits[i].TryGetComponent<IDamageable>(out var _target);

            if(_target == null)
            {
                continue;
            }
            switch (player.AttackMotion)
            {
                case 3:
                    {
                        player.Attack?.Invoke(_target, player.PlayerData.AttackStatus.ATK);
                        break;
                    }
                default:
                    {
                        player.Attack?.Invoke(_target, player.PlayerData.AttackStatus.ATK); // CombatManager에 연결해서 대미지 주는 함수 호출할 것.
                        return;
                    }
            }
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
        int _count = Physics2D.Raycast(player.PlayerTransform.position, player.GunDirection, player.PlayerData.AttackStatus.ContactFilter, player.RayHits);
        if (_count <= 0)
        {
            return;
        }

        for (int i = 0; i < _count; i++)
        {
            player.RayHits[i].transform.TryGetComponent<IDamageable>(out var _target);

            if (_target == null)
            {
                continue;
            }
            player.Attack?.Invoke(_target, player.PlayerData.AttackStatus.GunATK);
        }
    }
}

public class PlayerDamageState : IPlayerState
{
    private float currentTime;
    private int currentDamageState;
    private float deltaTime = Time.deltaTime;
    public void Enter(PlayerController player)
    {
        player.Animator.SetFloat(player.DamageStateHash, player.DamageRate);
        player.Animator.SetTrigger(player.DamageHash);
        if(player.DamageRate < 10f)
        {
            currentDamageState = 0;
        }
        else if(player.DamageRate < 20f)
        {
            currentDamageState = 1;
        }
        else
        {
            currentDamageState = 2;
        }
    }

    public void Execute(PlayerController player)
    {
        currentTime += deltaTime;
        if(currentTime >= player.StunTime[currentDamageState])
        {
            player.ChangeState(player.IdleState);
        }
    }

    public void Exit(PlayerController player)
    {
        currentTime = 0;
        currentDamageState = 0;
    }

    
}