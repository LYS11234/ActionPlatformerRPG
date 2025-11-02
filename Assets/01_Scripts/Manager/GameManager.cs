using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField]
    private CameraManager _cameraManager;
    public CameraManager CameraManager { get { return _cameraManager; } }
    [SerializeField]
    private PlayerManager _playerManager;

    public PlayerManager PlayerManager { get { return _playerManager; } }

    [SerializeField]
    private UIManager _uiManager;
    public UIManager UIManager { get { return _uiManager; } }

}
