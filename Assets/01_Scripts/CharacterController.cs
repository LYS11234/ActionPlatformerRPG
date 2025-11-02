using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Interface
    [SerializeField]
    protected ICharacterState characterState;
    #endregion
    #region States
    public ICharacterState IdleState { get; protected set; }
    public ICharacterState AttackState { get; protected set; }
    public ICharacterState MoveState { get; protected set; }
    public ICharacterState JumpState { get; protected set; }
    public ICharacterState FallState { get; protected set; }
    public ICharacterState CrouchState { get; protected set; }
    public ICharacterState SkillState { get; protected set; }
    public ICharacterState ShootState { get; protected set; }
    #endregion
    #region Audio
    [Header("Audio")]
    [SerializeField]
    protected AudioSource source;
    public AudioSource Source { get { return source; } }
    #region Audio Clips
    [SerializeField]
    protected AudioClip[] attackSounds;
    public AudioClip[] AttackSounds { get { return attackSounds; } }
    [SerializeField]
    protected AudioClip[] idleSounds;
    public AudioClip[] IdleSounds { get { return idleSounds; } }
    [SerializeField]
    protected AudioClip[] emotionSounds;
    public AudioClip[] EmotionSounds { get { return emotionSounds; } }
    [SerializeField]
    protected AudioClip[] jumpSounds;
    public AudioClip[] JumpSounds { get {return jumpSounds; } }
    #endregion Audio Clips
    #endregion Audio
    [Space(30)]
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
    protected int attackMotionLength;
    public bool IsSprinting { get; protected set; }
    public bool IsCrouching {  get; protected set; }
    public bool IsJumping {  get; protected set; }
    public Vector2 MoveInput { get; protected set; }
    public int IdleMotion {  get; protected set; }
    public float AttackMotion { get; protected set; }
    public bool IsAttack { get; protected set; }
    public bool CanAttack { get; protected set; }

    public bool IsShooting {  get; protected set; }

    public float HP { get; protected set; }
    [SerializeField]
    protected bool canAttack;
    [SerializeField]
    protected float currentTime = 5;

    [SerializeField]
    protected float dir;

    public Vector2 CrouchSize { get; protected set; }
    public Vector2 StandSize { get; protected set; }
    public Vector2 CrouchOffset {  get; protected set; }
    public Vector2 StandOffset { get; protected set; }
    #endregion

    #region State Hashes

    protected int idleHash = Animator.StringToHash("Idle");
    protected int motionsHash = Animator.StringToHash("Motions");
    protected int moveHash = Animator.StringToHash("Move");
    protected int attackHash = Animator.StringToHash("Attack");
    protected int isAttackHash = Animator.StringToHash("IsAttack");
    protected int shootHash = Animator.StringToHash("Shoot");
    protected int isShootHash = Animator.StringToHash("IsShoot");
    protected int jumpHash = Animator.StringToHash("Jump");
    protected int velYHash = Animator.StringToHash("VelY");
    protected int crouchHash = Animator.StringToHash("Crouch");

    #endregion

    #region Components
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected Rigidbody2D rigidBody;
    public Rigidbody2D RigidBody { get { return rigidBody; } }
    [SerializeField]
    protected Animator animator;
    public Animator Animator { get { return animator; } }
    [SerializeField]
    protected Transform characterTransform;
    [SerializeField]
    protected BoxCollider2D collider;
    public BoxCollider2D Collider { get { return collider; } }  

    [SerializeField]
    protected Vector2[] attackSizes;
    [SerializeField]
    protected Vector2[] attackPoses;
    [SerializeField]
    protected ContactFilter2D contactFilter;
    public bool IsJump { get { return Animator.GetBool(jumpHash); } }
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

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterTransform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider2D>();
        characterState = IdleState;
        CanAttack = true;
        characterState.Enter(this);
        IsWalk();
    }

    protected virtual void FixedUpdate()
    {
        characterState.Execute(this);
    }

    #region Common
    public void CheckDir()
    {
        if (MoveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (MoveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void ChangeState(ICharacterState _state)
    {
        characterState?.Exit(this);

        characterState = _state;
        characterState.Enter(this);
    }

    #endregion

    #region Idle
    protected void ChangeIdleMotion()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        IdleMotion = UnityEngine.Random.Range(1, 101);
        Animator.SetFloat(idleHash, IdleMotion % 2);
    }

    public void PlayIdleSound()
    {
        Source.clip = IdleSounds[(int)Animator.GetFloat(idleHash)];
        Source.Play();
    }
    #endregion

    #region Move
    public void IsRun()
    {
        currentMoveSpeed = moveSpeed * runSpeed + (moveSpeed * additionalSpeed);
        Animator.SetFloat(moveHash, 2);
    }

    public void IsWalk()
    {
        currentMoveSpeed = moveSpeed + (moveSpeed * additionalSpeed);
        Animator.SetFloat(moveHash, 1);
    }

    

    public void ResetMove()
    {
        Animator.SetFloat(moveHash, 0);
    }
    public void Move()
    {
        RigidBody.linearVelocityX = MoveInput.x * currentMoveSpeed;
    }

    #endregion

    #region Attack

    public virtual void Attack()
    {
        
    }

    protected void NextAttackMotion()
    {
        currentTime = 5;
        if (AttackMotion < attackMotionLength)
        {
            AttackMotion++;
            return;
        }
        AttackMotion = 1;

    }

    public void ResetAttackMotion()
    {
        AttackMotion = 0f;
    }

    public void ChangeCanAttack()
    {
        CanAttack = !CanAttack;
    }

    protected virtual void AttackCheck()
    {

    }

    public virtual void CheckBulletHit()
    {
        
    }

    public virtual void Fire()
    {
        

    }

    public void StartAttackTimer()
    {
        currentTime = 5;
    }

    public void CheckTick()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            return;
        }
        ResetAttackMotion();
    }
    #endregion

    #region Jump
    public virtual void StartJump()
    {
        if (Animator.GetBool(jumpHash))
        {
            return;
        }
        RigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    public virtual void Jump()
    {
        Animator.SetFloat(velYHash, RigidBody.linearVelocityY);
    }

    public void IsLand()
    {
        Animator.SetBool(jumpHash, false);
        Animator.SetFloat(velYHash, 0);
    }

    

    #endregion

    #region Crouch

    public void StartCrouch()
    {
        Animator.SetBool(crouchHash, true);
        Collider.size = CrouchSize;
        Collider.offset = CrouchOffset;
    }

    public void EndCrouch()
    {
        Animator.SetBool(crouchHash, false);
        Collider.size = StandSize;
        Collider.offset = StandOffset;
    }

    #endregion

    #region Damage
    public void GetDamage(float _damage)
    {
        HP -= _damage;
        Debug.Log(HP);
    }

    #endregion


    

    

   

    

    //public virtual void MoveView(Vector2 dir)
    //{

    //}

    

    
    
    


    //public virtual void ResetCamera()
    //{

    //}
}
