using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private TMP_Text bulletText;

    public void UpdateBullet(int _currentBullets)
    {
        bulletText.SetText("× {0:00}", _currentBullets);
    }
}
