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
        //character.MoveView(character.GetMoveInput());
        if (Mathf.Abs(character.GetMoveInput().x) > 0.1f)
        {
            character.ChangeState(character.MoveState);
            
            return;
        }
        if(character.IsShooting)
        {
            character.ChangeState(character.ShootState);
        }
        //character.ResetCamera();
        character.GetAnimator().SetFloat("Move", 0);
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
        if (Mathf.Abs(character.GetMoveInput().x) < 0.1f)
        {
            character.ChangeState(character.IdleState);
            return;
        }
        if (character.IsRunning())
        {
            character.IsRun();
            character.GetAnimator().SetFloat("Move", 2);
        }
        else
        {
            character.IsWalk();
            character.GetAnimator().SetFloat("Move", 1);
        }

        if(character.GetMoveInput().x < 0)
        {
            character.GetSpriteRenderer().flipX = true;
        }
        else if(character.GetMoveInput().x > 0)
        {
            character.GetSpriteRenderer().flipX = false;
        }

        character.Move();
    }

    public void Exit(CharacterController character)
    {
        character.GetAnimator().SetFloat("Move", 0);
    }
}


public class JumpState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        character.Jump();
        character.GetAnimator().SetBool("Jump", true);
        int num = Random.Range(0, character.JumpSounds.Length);
        character.Source.clip = character.JumpSounds[num];
        character.Source.Play();
    }

    public void Execute(CharacterController character)
    {
        character.CheckTick();
        character.GetAnimator().SetFloat("VelY", character.GetRigidBody().linearVelocityY);
        if(!character.GetAnimator().GetBool("Jump"))
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
        character.GetAnimator().SetBool("Crouch", true);
        character.GetCollider().size = character.CrouchSize;
        character.GetCollider().offset = character.CrouchOffset;
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
        character.GetAnimator().SetBool("Crouch", false);
        character.GetCollider().size = character.StandSize;
        character.GetCollider().offset = character.StandOffset;
    }
}

public class AttackState : ICharacterState
{
    public void Enter(CharacterController character)
    {
        character.NextAttackMotion();
        character.Source.clip = character.AttackSounds[(int)character.AttackMotion - 1];
        character.Source.Play();
        // 공격 함수 추가
        character.AttackCheck();
        character.GetAnimator().SetFloat("Attack", character.AttackMotion);
        character.GetAnimator().SetTrigger("IsAttack");
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
        character.ChangeCanAttack();
        character.GetAnimator().SetFloat("Shoot", character.GetMoveInput().y);
        character.GetAnimator().SetTrigger("IsShoot");
        character.CheckBulletHit();
        // 데미지 주는 함수 추가할 것.
    }

    public void Execute(CharacterController character)
    {
        
        
    }

    public void Exit(CharacterController character)
    {

    }
}