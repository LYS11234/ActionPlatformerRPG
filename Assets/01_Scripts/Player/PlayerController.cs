using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    #region Player Components
    [SerializeField]
    protected CameraManager playerCam;
    #endregion

    

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
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

    }

    public void OnShot(InputAction.CallbackContext context)
    {
        isShooting = context.ReadValueAsButton();
    }

    

    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.ReadValueAsButton();
    }
}
