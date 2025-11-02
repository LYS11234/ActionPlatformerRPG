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
    public CameraManager CameraManager;
    [SerializeField]
    private PlayerManager _playerManager;

    public PlayerManager PlayerManager
    {
        get { return _playerManager; }
    }
    public PlayerController PlayerController;

    [SerializeField]
    private UIManager _uiManager;
    public UIManager UIManager
    {
        get { return _uiManager; }
    }
   

    void Start()
    {
        CameraManager.SetTarget(PlayerController.transform);
        PlayerManager.SetController(PlayerController);
        PlayerController.OnShotFired += UIManager.UpdateBullet;
        PlayerController.StartUI(); 
    }


}
