using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
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
    public AudioClip[] JumpSounds { get { return jumpSounds; } }
    #endregion Audio Clips
    #endregion Audio
    [Space(30)]
    #region Status
    [SerializeField]
    protected float attackSpeed;
    public bool IsSprinting { get; protected set; }
    public bool IsCrouching { get; protected set; }
    public bool IsJumping { get; protected set; }
    public Vector2 MoveInput { get; protected set; }
    public int IdleMotion { get; protected set; }
    public float AttackMotion { get; protected set; }
    public bool IsAttack { get; protected set; }
    public bool CanAttack { get; protected set; }

    public bool IsShooting { get; protected set; }

    [SerializeField]
    protected bool canAttack;
    [SerializeField]
    protected float currentTime = 5;



    [SerializeField]
    protected float dir;

    public Vector2 CrouchSize { get; protected set; }
    public Vector2 StandSize { get; protected set; }
    public Vector2 CrouchOffset { get; protected set; }
    public Vector2 StandOffset { get; protected set; }
    #endregion

    #region State Hashes

    protected int idleHash = Animator.StringToHash("Idle");
    protected int motionsHash = Animator.StringToHash("Motions");

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
    private MoveElements moveElements;
    [SerializeField]
    private AttackElements attackElements;
    #endregion
    #region Player Components
    [SerializeField]
    protected CameraManager playerCam;
    #endregion
    [SerializeField]
    private int bulletCount;
    #region Update UI
    public event Action<int> OnShotFired;
    public event Action<float, float> UpdateHP;
    public event Action<float, float> UpdateMP;
    #endregion

    #region Player Classes
    public PlayerMoveController MoveController { get; private set; }
    public PlayerActionController ActionController { get; private set; }
    public Parameters Parameters { get; private set; }

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
        CanAttack = true;
        
        Parameters = new Parameters();
        MoveController = new PlayerMoveController();
        ActionController = new PlayerActionController();
        Init();
        StartUI();
        characterState.Enter(this);
        IsWalk();
    }

    private void Init()
    {
        ActionController.Init(transform, animator, currentTime, attackElements);
        MoveController.Init(spriteRenderer, animator, rigidBody, collider, ChangeState, FallState, IdleState, moveElements);
    }

    public void StartUI()
    {
        Parameters.Init(100, 100); //하드코딩은 Google Sheet에서 읽어오는 방식으로 교체 예정
        OnShotFired?.Invoke(bulletCount);
        UpdateHP?.Invoke(Parameters.MaxHP, Parameters.CurrentHP);
        UpdateMP?.Invoke(Parameters.MaxMP, Parameters.CurrentMP);
    }
    protected void FixedUpdate()
    {
        characterState.Execute(this);

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
        MoveInput = context.ReadValue<Vector2>();
        if (MoveInput.x < 0)
        {
            dir = -1;
            return;
        }
        else if (MoveInput.x > 0)
        {
            dir = 1;
            return;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        IsSprinting = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        IsJumping = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        IsAttack = context.ReadValueAsButton();
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        if(bulletCount <= 0)
        {
            IsShooting = false;
            return;
        }
        if(!canAttack)
        {
            IsShooting = false;
            return;
        }
        IsShooting = context.ReadValueAsButton();
    }

    public void ShotBullet()
    {
        bulletCount--;
        OnShotFired.Invoke(bulletCount);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        IsCrouching = context.ReadValueAsButton();
    }

    public void SetIdleState()
    {
        ChangeCanAttack();
        ChangeState(IdleState);
    }

    #region Common
    public void CheckDir()
    {
        MoveController.CheckDir(MoveInput);
    }

    public void ChangeState(ICharacterState _state)
    {
        characterState?.Exit(this);

        characterState = _state;
        characterState.Enter(this);
    }

    public void CheckState()
    {
        if (RigidBody.linearVelocityY < 0)
        {
            ChangeState(FallState);
        }
        if (IsAttack && ActionController.CanAttack)
        {
            ChangeCanAttack();
            ChangeState(AttackState);
        }
        if (IsCrouching)
        {
            ChangeState(CrouchState);
            return;
        }
        if (IsJumping)
        {
            //character.ResetCamera();
            ChangeState(JumpState);
            return;
        }
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
        MoveController.IsRun();
    }

    public void IsWalk()
    {
        MoveController.IsWalk();
    }

    public void ResetMove()
    {
        MoveController.ResetMove();
    }
    public void Move()
    {
        MoveController.Move(MoveInput);
    }

    #endregion
    #region Attack

    public void Attack()
    {
        ActionController.Attack(dir);
        Source.clip = AttackSounds[(int)ActionController.AttackMotion - 1];
        Source.Play();
    }

    public void CheckTick()
    {
        ActionController.CheckComboTime();
    }
    public void ChangeCanAttack()
    {
        ActionController.ChangeCanAttack();
    }

    

    public void Fire()
    {
        ActionController.Fire(dir, MoveInput);
    }

    #endregion

    #region Jump
    public void StartJump()
    {
        if (MoveController.IsJump)
        {
            return;
        }
        MoveController.StartJump(); 
        int num = UnityEngine.Random.Range(0, JumpSounds.Length);
        Source.clip = JumpSounds[num];
        Source.Play();
    }

    public void Jump()
    {
        MoveController.Jump();
    }

    public void Fall()
    {
        MoveController.Fall();
    }

    public void IsLand()
    {
        MoveController.IsLand();
    }
    #endregion
    #region Crouch

    public void StartCrouch()
    {
        MoveController.StartCrouch();
    }

    public void EndCrouch()
    {
        MoveController.EndCrouch();
    }

    #endregion

    #region Damage
    public void GetDamage(float _damage)
    {
        Parameters.UpdateCurrentHP(-_damage);
        Debug.Log(Parameters.CurrentHP);
    }

    #endregion
}
