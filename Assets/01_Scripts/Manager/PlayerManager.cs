using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerControls playerControls;

    public void SetController(PlayerController controller)
    {
        if(playerControls.IsUnityNull())
        {
            playerControls = new PlayerControls();
        }
        playerControls.Enable();
        playerControls.PlayerActions.Enable();
        playerControls.PlayerActions.Move.performed += controller.OnMove;
        playerControls.PlayerActions.Move.canceled += controller.OnMove;
        playerControls.PlayerActions.Jump.performed += controller.OnJump;
        playerControls.PlayerActions.Jump.canceled += controller.OnJump;
        playerControls.PlayerActions.Sprint.performed += controller.OnSprint;
        playerControls.PlayerActions.Sprint.canceled += controller.OnSprint;
        playerControls.PlayerActions.Crouch.performed += controller.OnCrouch;
        playerControls.PlayerActions.Crouch.canceled += controller.OnCrouch;
        playerControls.PlayerActions.Shot.performed += controller.OnShot;
        playerControls.PlayerActions.Shot.canceled += controller.OnShot;
        playerControls.PlayerActions.Attack.performed += controller.OnAttack;
        playerControls.PlayerActions.Attack.canceled += controller.OnAttack;
    }
}
