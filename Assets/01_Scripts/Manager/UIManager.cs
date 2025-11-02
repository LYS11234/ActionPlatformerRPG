using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private TMP_Text bulletText;

    public void UpdateBullet(uint _currentBullets)
    {
        bulletText.text = $"× {_currentBullets.ToString("D2")}";
    }
   
}
