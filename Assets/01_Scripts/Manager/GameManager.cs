using UnityEngine;
using Excel;
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

    [field:SerializeField]
    public CameraManager CameraManager { get; private set; }
    [field:SerializeField]
    public PlayerManager PlayerManager { get; private set; }

    [field:SerializeField]
    public UIManager UIManager { get; private set; }
    [field:SerializeField]
    public CombatManager CombatManager { get; private set; }
    [SerializeField]
    private GoogleSpreadSheetManager googleSpreadSheetManager;
    [SerializeField]
    private DataManager dataManager;

    private void Start()
    {
        googleSpreadSheetManager.FetchGoogleSheet();
    }

}
