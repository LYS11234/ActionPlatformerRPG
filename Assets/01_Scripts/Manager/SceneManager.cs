using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private BoxCollider2D bound;
    void Start()
    {
        player.OnShotFired += GameManager.Instance.UIManager.UpdateBullet;
        player.UpdateHP += GameManager.Instance.UIManager.UpdateHP;
        player.UpdateMP += GameManager.Instance.UIManager.UpdateMP;
        player.Attack += GameManager.Instance.CombatManager.CalculateDamage;
        GameManager.Instance.CameraManager.SetTarget(player.transform);
        GameManager.Instance.CameraManager.SetBound(bound);
        GameManager.Instance.PlayerManager.SetController(player);
        
    }
#if UNITY_EDITOR
    [ContextMenu("Damage 9")]
    private void Test9Damage()
    {
        GameManager.Instance.CombatManager.CalculateDamage(player, 9);
    }

    [ContextMenu("Damage 19")]
    private void Test19Damage()
    {
        GameManager.Instance.CombatManager.CalculateDamage(player, 19);
    }

    [ContextMenu("Damage 30")]
    private void Test30Damage()
    {
        GameManager.Instance.CombatManager.CalculateDamage(player, 30);
    }
#endif
}
