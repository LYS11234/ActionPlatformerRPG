using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AttackElements
{
    public Vector2[] AttackSizes;
    public Vector2[] AttackPoses;
    public int AttackMotionLength;
    public ContactFilter2D ContactFilter;
}

public class PlayerActionController
{
    private Animator animator;
    private List<CharacterController> hitList = new List<CharacterController>();
    private RaycastHit2D[] rayHits = new RaycastHit2D[byte.MaxValue];
    private Collider2D[] overlapHits = new Collider2D[byte.MaxValue];
    public Transform CharacterTransform { get; private set; }

    private float dir;
    public bool CanAttack { get; private set; }
    public float AttackMotion { get; private set; }
    #region Hashes
    private int attackHash = Animator.StringToHash("Attack");
    private int isAttackHash = Animator.StringToHash("IsAttack");
    protected int shootHash = Animator.StringToHash("Shoot");
    private int isShootHash = Animator.StringToHash("IsShoot");
    #endregion
    private float currentTime;
    private Vector2 gunDirection = Vector2.zero;


    private AttackElements elements;
    public void Init(Transform _characterTransform, Animator _animator, float _currentTime, AttackElements _attackElements)
    {
        CharacterTransform = _characterTransform;
        animator = _animator;
        elements = _attackElements;
        currentTime = _currentTime;
        CanAttack = true;
    }


    public void ChangeCanAttack()
    {
        CanAttack = !CanAttack;
    }

    #region Attack
    public void Attack(float _dir)
    {
        dir = _dir;
        NextAttackMotion();
        AttackCheck();
        animator.SetFloat(attackHash, AttackMotion);
        animator.SetTrigger(isAttackHash);
    }

    private void AttackCheck()
    {
        int num = (int)AttackMotion - 1;
        Vector2 attackPos = new Vector2(CharacterTransform.position.x + elements.AttackPoses[num].x * dir, CharacterTransform.position.y + elements.AttackPoses[num].y);

        int _count = Physics2D.OverlapBox(attackPos, elements.AttackSizes[num], 0, elements.ContactFilter, overlapHits);

        for (int i = 0; i < _count; i++)
        {
            if (!overlapHits[i].TryGetComponent<CharacterController>(out CharacterController hitChar))
            {
                continue;
            }
            hitList.Add(hitChar);
        }

        if (hitList.Count <= 0)
        {
            return;
        }
        switch (AttackMotion)
        {
            case 3:
                {
                    ThirdAttack(hitList);
                    break;
                }
            default:
                {
                    hitList[0].GetDamage(1);
                    break;
                }
        }
        hitList.Clear();

    }

    private void ThirdAttack(List<CharacterController> hits)
    {
        for (int i = 0; i < hits.Count; i++)
        {
            hits[i].GetDamage(1);
        }
    }

    private void NextAttackMotion()
    {
        currentTime = 5;
        if (AttackMotion < elements.AttackMotionLength)
        {
            AttackMotion++;
            return;
        }
        AttackMotion = 1;
    }

    public void CheckComboTime()
    {
        if(currentTime <= 0)
        {
            AttackMotion = 0f;
            return;
        }
        currentTime -= Time.deltaTime;
    }

    #endregion
    #region Shot
    public void CheckBulletHit(Vector2 _moveInput)
    {
        gunDirection.x = dir;
        if (_moveInput.y < 0)
        {
            gunDirection.y = -1;
        }
        else if (_moveInput.y > 0)
        {
            gunDirection.y = 1;
        }
        else
        {
            gunDirection.y = 0;
        }
        int _count = Physics2D.Raycast(CharacterTransform.position, gunDirection, elements.ContactFilter, rayHits, 100);

        for (int i = 0; i < _count; i++)
        {
            if (!rayHits[i].transform.TryGetComponent<CharacterController>(out CharacterController hitChar))
            {
                continue;
            }
            hitList.Add(hitChar);
        }
        if (hitList.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < hitList.Count; i++)
        {
            hitList[i].GetDamage(2);
        }
        hitList.Clear();
    }

    public void Fire(float _dir, Vector2 _moveInput)
    {
        dir = _dir;
        ChangeCanAttack();
        animator.SetFloat(shootHash, _moveInput.y);
        animator.SetTrigger(isShootHash);
        CheckBulletHit(_moveInput);
    }
    #endregion
}
