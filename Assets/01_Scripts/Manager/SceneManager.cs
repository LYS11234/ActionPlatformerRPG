using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    void Start()
    {
        player.OnShotFired += GameManager.Instance.UIManager.UpdateBullet;
        GameManager.Instance.CameraManager.SetTarget(player.transform);
        GameManager.Instance.PlayerManager.SetController(player);
    }

}
