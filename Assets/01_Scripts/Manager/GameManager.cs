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
    public static PlayerManager PlayerManager;
    public PlayerController PlayerController;

   

    void Start()
    {
        CameraManager.SetTarget(PlayerController.transform);
        PlayerManager = GetComponentInChildren<PlayerManager>();
        PlayerManager.SetController(PlayerController);
    }


}
