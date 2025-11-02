using UnityEngine;

public interface ICharacterState
{
    void Enter(CharacterController character);
    void Execute(CharacterController character);
    void Exit(CharacterController character);

}


public class IdleState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        
    }

    public void Execute(CharacterController character)
    {
        character.CheckTick();
        if (character.IsAttack && character.CanAttack)
        {
            character.ChangeCanAttack();
            character.ChangeState(character.AttackState);
        }
        if (character.IsCrouching)
        {
            character.ChangeState(character.CrouchState);
            return;
        }
        if(character.IsJumping)
        {
            //character.ResetCamera();
            character.ChangeState(character.JumpState);
            return;
        }
        //character.MoveView(character.MoveInput);
        if (Mathf.Abs(character.MoveInput.x) > 0.1f)
        {
            character.ChangeState(character.MoveState);
            
            return;
        }
        if(character.IsShooting)
        {
            character.ChangeState(character.ShootState);
        }
        //character.ResetCamera();
        character.ResetMove();
    }

    public void Exit(CharacterController character)
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
        character.CheckTick();
        if (character.IsAttack && character.CanAttack)
        {
            character.ChangeCanAttack();
            character.ChangeState(character.AttackState);
        }
        if (character.IsCrouching)
        {
            character.ChangeState(character.CrouchState);
            return;
        }
        if (character.IsJumping)
        {
            character.ChangeState(character.JumpState);
            return;
        }
        if (Mathf.Abs(character.MoveInput.x) < 0.1f)
        {
            character.ChangeState(character.IdleState);
            return;
        }
        if (character.IsSprinting)
        {
            character.IsRun();
        }
        else
        {
            character.IsWalk();
        }

        character.CheckDir();

        character.Move();
    }

    public void Exit(CharacterController character)
    {
        character.ResetMove();
    }
}


public class JumpState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        character.StartJump();
    }

    public void Execute(CharacterController character)
    {
        character.CheckTick();
        character.Jump();
        if (!character.IsJump)
        {
            character.ChangeState(character.IdleState);
        }
    }

    public void Exit(CharacterController character)
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
        character.CheckTick();
    }

    public void Exit(CharacterController character)
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
        character.CheckTick();
        if (character.IsCrouching)
        {
            return;
        }
        character.ChangeState(character.IdleState);
    }

    public void Exit(CharacterController character)
    {
        character.EndCrouch();
    }
}

public class AttackState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        character.Attack();
    }

    public void Execute(CharacterController character)
    {

    }

    public void Exit(CharacterController character)
    {
        character.StartAttackTimer();
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
}