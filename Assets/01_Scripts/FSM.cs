using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public interface ICharacterState
{
    void Enter(CharacterController character);
    void Execute(CharacterController character);
    void Exit(CharacterController character);

    void Enter(PlayerController player);
    void Execute(PlayerController player);
    void Exit(PlayerController player);
}


public class IdleState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        
    }

    public void Execute(CharacterController character)
    {
        
    }

    public void Exit(CharacterController character)
    {
        
    }

    public void Enter(PlayerController player)
    {

    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.CheckState();
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
        //player.ResetCamera();
        player.ResetMove();
    }

    public void Exit(PlayerController player)
    {

    }
}

public class MoveState : ICharacterState
{
    public void Enter(CharacterController character)
    {

    }

    public void Execute(CharacterController character)
    {
        
    }

    public void Exit(CharacterController character)
    {
        character.ResetMove();
    }

    public void Enter(PlayerController player)
    {

    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.CheckState();
        if (Mathf.Abs(player.MoveInput.x) < 0.1f)
        {
            player.ChangeState(player.IdleState);
            return;
        }
        if (player.IsSprinting)
        {
            player.IsRun();
        }
        else
        {
            player.IsWalk();
        }

        player.CheckDir();

        player.Move();
    }
    public void Exit(PlayerController player)
    {
        player.ResetMove();
    }
}


public class JumpState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        
    }

    public void Execute(CharacterController character)
    {

    }

    public void Exit(CharacterController character)
    {
        
    }
    public void Enter(PlayerController player)
    {
        player.StartJump();
    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.Jump();
    }
    public void Exit(PlayerController player)
    {

    }
}

public class FallState : ICharacterState
{
    public void Enter(CharacterController character)
    {

    }

    public void Execute(CharacterController character)
    {

    }

    public void Exit(CharacterController character)
    {

    }

    public void Enter (PlayerController player)
    {

    }

    public void Execute(PlayerController player)
    {
        player.CheckTick();
        player.Fall();

    }

    public void Exit(PlayerController player)
    {

    }
}

public class CrouchState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        character.StartCrouch();
    }

    public void Execute(CharacterController character)
    {
        
    }

    public void Exit(CharacterController character)
    {
        character.EndCrouch();
    }
    public void Enter(PlayerController player)
    {
        player.StartCrouch();
    }
    public void Execute(PlayerController player)
    {
        player.CheckTick();
        if(player.IsCrouching)
        {
            return;
        }
        player.ChangeState(player.IdleState);
    }
    public void Exit(PlayerController player)
    {
        player.EndCrouch();
    }
}

public class AttackState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        
    }

    public void Execute(CharacterController character)
    {

    }

    public void Exit(CharacterController character)
    {
        
    }
    public void Enter(PlayerController player)
    {
        player.Attack();
    }
    public void Execute(PlayerController player)
    {

    }
    public void Exit(PlayerController player)
    {

    }
}
public class SkillState : ICharacterState
{
    public void Enter(CharacterController character)
    {
       
    }

    public void Execute(CharacterController character)
    {
        
    }

    public void Exit(CharacterController character)
    {

    }
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

public class ShootState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        character.Fire();
        // 데미지 주는 함수 추가할 것.
    }

    public void Execute(CharacterController character)
    {
        
        
    }

    public void Exit(CharacterController character)
    {

    }
    public void Enter(PlayerController player)
    {
        player.Fire();
    }
    public void Execute(PlayerController player)
    {

    }
    public void Exit(PlayerController player)
    {

    }
}