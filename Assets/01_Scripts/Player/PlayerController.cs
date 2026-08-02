using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    #region Components
    [Header("Components")]
    [field: SerializeField]
    protected SpriteRenderer spriteRenderer;
    [field: SerializeField]
    public Transform PlayerTransform { get; private set; }
    [field: SerializeField]
    public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField]
    public Animator Animator { get; private set; }
    [field: SerializeField]
    public BoxCollider2D Collider { get; private set; }
    [SerializeField]
    private CameraManager playerCam;
    #endregion
    #region Audio
    [Header("Audio")]
    [field: SerializeField]
    private AudioSource source;
    #region Audio Clips
    [field:SerializeField]
    public AudioClip[] AttackSounds { get; private set; }
    [field:SerializeField]
    public AudioClip[] IdleSounds { get; private set; }
    [field:SerializeField]
    public AudioClip[] EmotionSounds { get; private set; }
    [field:SerializeField]
    public AudioClip[] jumpSounds { get; private set; }
    #endregion Audio Clips
    #endregion Audio
    [field:Space(10)]
    #region Status
    [field:Header("Status")]
    public PlayerInfo PlayerData;
    [field:SerializeField]
    public float[] StunTime { get; private set; }
    #endregion
    [field: Space(10)]
    // 인스펙터에서 할당하는 것을 더 선호.
    #region States
    private IPlayerState currentState;
    public IPlayerState IdleState { get; private set; }
    public IPlayerState AttackState { get; private set; }
    public IPlayerState MoveState { get; private set; }
    public IPlayerState JumpState { get; private set; }
    public IPlayerState FallState { get; private set; }
    public IPlayerState CrouchState { get; private set; }
    public IPlayerState SkillState { get; private set; }
    public IPlayerState ShootState { get; private set; }
    public IPlayerState DamageState { get; private set; }
    #endregion

    [HideInInspector]
    public Collider2D[] OverlapHits = new Collider2D[byte.MaxValue];
    [HideInInspector]
    public RaycastHit2D[] RayHits = new RaycastHit2D[byte.MaxValue];
    [HideInInspector]
    public Vector2 GunDirection = Vector2.zero;
    protected bool canAttack;
    [HideInInspector]
    public float CurrentTime = 5;
    [HideInInspector]
    public float AttackMotion;
    [HideInInspector]
    private int bulletCount;
    
    public int idleCount;
    

    [field: SerializeField]
    public float Dir {  get; private set; }
    public float DamageRate { get; private set; }

    #region Input System
    // 나중에 InputManager.cs 로 옮기기.
    protected float attackSpeed;
    public bool IsSprinting { get; protected set; }
    public bool IsCrouching { get; protected set; }
    public bool IsJumping { get; protected set; }
    public Vector2 MoveInput { get; protected set; }
    public int IdleMotion { get; protected set; }
    public bool IsAttack { get; protected set; }
    public bool CanAttack { get; protected set; }

    public bool IsShooting { get; protected set; }
    #endregion
    #region State Hashes
    public readonly int IdleHash = Animator.StringToHash("Idle");
    public readonly int MotionsHash = Animator.StringToHash("Motions");
    public readonly int MoveHash = Animator.StringToHash("Move");
    public readonly int VelYHash = Animator.StringToHash("VelY");
    public readonly int JumpHash = Animator.StringToHash("Jump");
    public readonly int CrouchHash = Animator.StringToHash("Crouch");
    public readonly int AttackHash = Animator.StringToHash("Attack");
    public readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    public readonly int ShootHash = Animator.StringToHash("Shoot");
    public readonly int IsShootHash = Animator.StringToHash("IsShoot");
    public readonly int DamageHash = Animator.StringToHash("Damage");
    public readonly int DamageStateHash = Animator.StringToHash("DamageState");
    public readonly int IdleCountHash = Animator.StringToHash("IdleCount");

    public readonly int IdleStateHash = Animator.StringToHash("Unitychan_Idle");
    #endregion



    
    public bool IsJump { get { return Animator.GetBool(JumpHash); } }
    #region Update UI
    public event Action<int> OnShotFired;
    public event Action<float, float> UpdateHP;
    public event Action<float, float> UpdateMP;
    #endregion
    #region Take Damage or Give Damage
    public Action<IDamageable, float> Attack;
    #endregion

    protected void Awake()
    {
        IdleState = new PlayerIdleState();
        AttackState = new PlayerAttackState();
        MoveState = new PlayerMoveState();
        JumpState = new PlayerJumpState();
        FallState = new PlayerFallState();
        CrouchState = new PlayerCrouchState();
        SkillState = new PlayerSkillState();
        ShootState = new ShootState();
        DamageState = new PlayerDamageState();
    }

    protected void Start()
    {
        //인스펙터로 집어넣자
        currentState = IdleState;
        CanAttack = true;
        PlayerData.MoveStatus.StandOffset = new Vector2(Collider.offset.x, Collider.offset.y);
        PlayerData.MoveStatus.StandSize = new Vector2(Collider.size.x, Collider.size.y);
        PlayerData.MoveStatus.CrouchOffset = new Vector2(Collider.offset.x, 0.7457407f);
        PlayerData.MoveStatus.CrouchSize = new Vector2(Collider.size.x, 1.513318f);
        PlayerData.Parameters = new Parameters();
        StartUI();
        currentState.Enter(this);
    }

    public void StartUI()
    {
        PlayerData.Parameters.Init(100, 100); //하드코딩은 Google Sheet에서 읽어오는 방식으로 교체 예정
        OnShotFired?.Invoke(bulletCount);
        UpdateHP?.Invoke(PlayerData.Parameters.MaxHP, PlayerData.Parameters.CurrentHP);
        UpdateMP?.Invoke(PlayerData.Parameters.MaxMP, PlayerData.Parameters.CurrentMP);
    }
    protected void FixedUpdate()
    {
        currentState.Execute(this);

    }

    //public override void MoveView(Vector2 Dir)
    //{
    //    base.MoveView(Dir);
    //    playerCam.SetView(new Vector3(characterTransform.position.x, characterTransform.position.y + 3 * Dir.y, characterTransform.position.z));
    //}

    //public override void ResetCamera()
    //{
    //    base.ResetCamera();
    //    playerCam.SetTarget(characterTransform);
    //}

    #region Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        if (MoveInput.x < 0)
        {
            Dir = -1;
            return;
        }
        else if (MoveInput.x > 0)
        {
            Dir = 1;
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
    #endregion
    public void SetIdleState()
    {
        ChangeCanAttack();
        ChangeState(IdleState);
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

    public void ChangeState(IPlayerState _state)
    {
        currentState?.Exit(this);

        currentState = _state;
        Debug.Log($"Current State: {currentState}");
        currentState.Enter(this);
    }

    public bool CheckState()
    {
        if (RigidBody.linearVelocityY < -0.1f)
        {
            ChangeState(FallState);
            return true;
        }
        if (IsAttack && CanAttack)
        {
            ChangeCanAttack();
            ChangeState(AttackState);
            return true;
        }
        if (IsCrouching)
        {
            ChangeState(CrouchState);
            return true;
        }
        if (IsJumping)
        {
            //character.ResetCamera();
            ChangeState(JumpState);
            return true;
        }
        return false;
    }

    public void IdleCount()
    {
        idleCount++;
        Animator.SetInteger(IdleCountHash, idleCount);
    }

    #endregion

    public void PlaySound(AudioClip _audioClip)
    {
        source.clip = _audioClip;
        source.Play();
    }

    #region Idle
    public void ChangeIdleMotion()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        IdleMotion = UnityEngine.Random.Range(1, 101);
        Animator.SetFloat(IdleHash, IdleMotion % 2);
    }

    private void PlayIdleSound()
    {
        source.clip = IdleSounds[(int)Animator.GetFloat(IdleHash)];
        source.Play();
    }
    #endregion
    #region Attack


    public void CheckTick()
    {
        if (CurrentTime <= 0)
        {
            AttackMotion = 0f;
            return;
        }
        CurrentTime -= Time.deltaTime;
    }
    public void ChangeCanAttack()
    {
        CanAttack = !CanAttack;
    }

    


    #endregion

    #region Jump
    public void IsLand()
    {
        Animator.SetBool(JumpHash, false);
        Animator.SetFloat(VelYHash, 0);
        ChangeState(IdleState);
    }
    #endregion
    #region Damage
    public void TakeDamage(float _damage)
    {
        PlayerData.Parameters.UpdateCurrentHP(-_damage);
        DamageRate = _damage / PlayerData.Parameters.MaxHP * 100f;

        if (currentState != DamageState)
        {
            ChangeState(DamageState);
        }
        UpdateHP(PlayerData.Parameters.MaxHP, PlayerData.Parameters.CurrentHP);
        Debug.Log(PlayerData.Parameters.CurrentHP);
    }

    #endregion




#if UNITY_EDITOR
    #region Debug
    private void OnDrawGizmos()
    {
        if (PlayerData.AttackStatus == null || PlayerData.AttackStatus.AttackHitBoxes == null)
        {
            return;
        }
        Gizmos.color = UnityEngine.Color.red;
        foreach (var box in PlayerData.AttackStatus.AttackHitBoxes)
        {
            Vector3 globalPos = transform.position + (Vector3)box.Offset;
            Gizmos.DrawWireCube(globalPos, box.Size);
        }
    }
    #endregion
#endif

}
