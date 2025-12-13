using UnityEngine;

public class EnemyManager : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Parameters parameters;

    public void TakeDamage(float _damage)
    {
        parameters.UpdateCurrentHP(-_damage);
        Debug.Log(parameters.CurrentHP);
    }
}
