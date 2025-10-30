using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    #region Interface
    [SerializeField]
    protected ICharacterState characterState;
    #endregion
    #region States
    public ICharacterState IdleState { get; private set; }
    public ICharacterState AttackState { get; private set; }
    public ICharacterState MoveState { get; private set; }
    public ICharacterState JumpState { get; private set; }
    public ICharacterState FallState { get; private set; }
    public ICharacterState CrouchState { get; private set; }
    public ICharacterState SkillState { get; private set; }
    public ICharacterState ShootState { get; private set; }
    #endregion
    #region Status
    [SerializeField]
    protected float currentMoveSpeed;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected float runSpeed;
    [SerializeField]
    protected float additionalSpeed;
    [SerializeField]
    protected float attackSpeed;
    [SerializeField]
    protected float jumpForce;
    [SerializeField]
    protected bool isSprinting;
    [SerializeField]
    protected bool isCrouching;
    [SerializeField]
    protected bool isJumping;
    protected Vector2 moveInput;
    [SerializeField]
    protected int idleMotion;
    [SerializeField]
    protected float attackMotion;
    [SerializeField]
    protected int attackCount;
    protected bool isShooting;
    #endregion

    #region Components
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Rigidbody2D rigidBody;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    protected Transform characterTransform;
    [SerializeField]
    protected BoxCollider2D collider;

    #endregion
    
    protected void Awake()
    {
        IdleState = new IdleState();
        AttackState = new AttackState();
        MoveState = new MoveState();
        JumpState = new JumpState();
        FallState = new FallState();
        CrouchState = new CrouchState();
        SkillState = new SkillState();
        ShootState = new ShootState();
    }

    protected void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterTransform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider2D>();
        characterState = IdleState;
        characterState.Enter(this);
        IsWalk();
    }

    protected virtual void FixedUpdate()
    {
        characterState.Execute(this);
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public Rigidbody2D GetRigidBody()
    {
        return rigidBody;
    }

    public float GetIdleMotion()
    {
        return idleMotion;
    }

    protected void ChangeIdleMotion()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        idleMotion = UnityEngine.Random.Range(1, 101);
        animator.SetFloat("Idle", idleMotion % 2);
    }
    public void IsRun()
    {
        currentMoveSpeed = moveSpeed * runSpeed + (moveSpeed * additionalSpeed);
    }

    public void IsWalk()
    {
        currentMoveSpeed = moveSpeed + (moveSpeed * additionalSpeed);
    }

    public bool IsRunning()
    {
        return isSprinting;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public BoxCollider2D GetCollider()
    {
        return collider;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    public void ChangeState(ICharacterState _state)
    {
        characterState?.Exit(this);

        characterState = _state;
        characterState.Enter(this);
    }

    public void Move()
    {
        rigidBody.linearVelocityX = moveInput.x * currentMoveSpeed;
    }

    public void Jump()
    {
        if(animator.GetBool("Jump"))
        {
            return;
        }
        rigidBody.AddForceY(jumpForce);
    }

    //public virtual void MoveView(Vector2 dir)
    //{

    //}

    public void IsLand()
    {
        animator.SetBool("Jump", false);
        animator.SetFloat("VelY", 0);
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public float GetAttackMotion()
    {
        return attackMotion;
    }

    public void ResetAttackMotion()
    {
        attackMotion = 0f;
    }

    //public virtual void ResetCamera()
    //{

    //}
    public bool IsShooting()
    {
        return isShooting;
    }
}
