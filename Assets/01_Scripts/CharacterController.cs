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

    [SerializeField]
    public Vector2 CrouchSize { get; protected set; }
    [SerializeField]
    public Vector2 StandSize { get; protected set; }
    [SerializeField]
    public Vector2 CrouchOffset {  get; protected set; }
    [SerializeField]
    public Vector2 StandOffset { get; protected set; }
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
    protected BoxCollider2D boxCollider;

    [SerializeField]
    protected Vector2[] attackSizes;
    [SerializeField]
    protected Vector2[] attackPoses;
    [SerializeField]
    protected LayerMask hitLayer;

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
        boxCollider = GetComponent<BoxCollider2D>();
        characterState = IdleState;
        CanAttack = true;
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


    protected void ChangeIdleMotion()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        IdleMotion = UnityEngine.Random.Range(1, 101);
        animator.SetFloat("Idle", IdleMotion % 2);
    }

    public void PlayIdleSound()
    {
        Source.clip = IdleSounds[(int)animator.GetFloat("Idle")];
        Source.Play();
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
        return IsSprinting;
    }

    public Vector2 GetMoveInput()
    {
        return MoveInput;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public BoxCollider2D GetCollider()
    {
        return boxCollider;
    }

    public void ChangeState(ICharacterState _state)
    {
        characterState?.Exit(this);

        characterState = _state;
        characterState.Enter(this);
    }

    public void Move()
    {
        rigidBody.linearVelocityX = MoveInput.x * currentMoveSpeed;
    }

    public void Jump()
    {
        if(animator.GetBool("Jump"))
        {
            return;
        }
        rigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    public void NextAttackMotion()
    {
        currentTime = 5;
        if (AttackMotion < attackMotionLength)
        {
            AttackMotion++;
            return;
        }
        AttackMotion = 1;

    }

    //public virtual void MoveView(Vector2 dir)
    //{

    //}

    public void IsLand()
    {
        animator.SetBool("Jump", false);
        animator.SetFloat("VelY", 0);
    }

    public void ResetAttackMotion()
    {
        AttackMotion = 0f;
    }

    public void ChangeCanAttack()
    {
        CanAttack = !CanAttack;
    }
    
    public virtual void AttackCheck()
    {
        
    }

    public void CheckBulletHit()
    {
        Vector2 _dir = Vector2.zero;
        if(MoveInput.y < 0)
        {
            _dir = new Vector2(dir, -1);
        }
        else if(MoveInput.y > 0)
        {
            _dir = new Vector2(dir, 1);
        }
        else
        {
            _dir = new Vector2(dir, 0);
        }
        RaycastHit2D[] hits = Physics2D.RaycastAll(characterTransform.position, _dir, 100, hitLayer);
        int num = 0;
        CharacterController[] hitChar = new CharacterController[hits.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].transform.TryGetComponent<CharacterController>(out hitChar[num]))
            {
                continue;
            }
            num++;
        }
        if (num > 0)
        {
            hitChar[0].GetDamage(2);
        }
    }

    public void GetDamage(float _damage)
    {
        HP -= _damage;
        Debug.Log(HP);
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
    //public virtual void ResetCamera()
    //{

    //}
}
