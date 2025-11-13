using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    void Start()
    {
        player.OnShotFired += GameManager.Instance.UIManager.UpdateBullet;
        player.UpdateHP += GameManager.Instance.UIManager.UpdateHP;
        player.UpdateMP += GameManager.Instance.UIManager.UpdateMP;
        GameManager.Instance.CameraManager.SetTarget(player.transform);
        GameManager.Instance.PlayerManager.SetController(player);
    }

}
