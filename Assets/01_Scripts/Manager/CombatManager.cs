using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public class CombatManager : MonoBehaviour
{
    public void CalculateDamage(IDamageable _target, float damage)
    {
        _target.TakeDamage(damage);
    }



}
