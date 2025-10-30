using JetBrains.Annotations;
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
        if(character.IsCrouching())
        {
            character.ChangeState(character.CrouchState);
            return;
        }
        if(character.IsJumping())
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
        if(character.IsShooting())
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
        Debug.Log("d");
        if (character.IsCrouching())
        {
            character.ChangeState(character.CrouchState);
            return;
        }
        if (character.IsJumping())
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
        
    }

    public void Execute(CharacterController character)
    {
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
        character.GetCollider().size = new Vector2(character.GetCollider().size.x, character.GetCollider().size.y * 0.5f);
        character.GetCollider().offset = new Vector2(character.GetCollider().offset.x, -0.5f);
    }

    public void Execute(CharacterController character)
    {
        if (character.IsCrouching())
        {
            return;
        }
        character.ChangeState(character.IdleState);
    }

    public void Exit(CharacterController character)
    {
        character.GetAnimator().SetBool("Crouch", false);
        character.GetCollider().size = new Vector2(character.GetCollider().size.x, character.GetCollider().size.y * 2f);
        character.GetCollider().offset = new Vector2(character.GetCollider().offset.x, -0.1280699f);
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

    }

    public void Execute(CharacterController character)
    {
        character.GetAnimator().SetFloat("Shoot", character.GetMoveInput().y);
        character.GetAnimator().SetTrigger("IsShoot");
        character.ChangeState(character.IdleState);
    }

    public void Exit(CharacterController character)
    {

    }
}