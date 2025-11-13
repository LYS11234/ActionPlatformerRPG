using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region HP
    [Header("HP")]
    [SerializeField]
    private TMP_Text hpText;
    [SerializeField]
    private Slider hpSlider;
    [Space(20)]
    #endregion
    #region MP
    [Header("MP")]
    [SerializeField]
    private TMP_Text mpText;
    [SerializeField]
    private Slider mpSlider;
    [Space(20)]
    #endregion

    #region Bullet
    [Header("BulletUI")]
    [SerializeField]
    private TMP_Text bulletText;
    #endregion
    public void UpdateBullet(int _currentBullets)
    {
        bulletText.SetText("× {0:00}", _currentBullets);
    }
    #region HP UI

    public void UpdateHP(float _maxHP, float _currentHP)
    {
        hpText.SetText("{0:0}/{1:0}", _currentHP, _maxHP);
        hpSlider.value = (_currentHP / _maxHP);
    }
    #endregion

    #region MP UI
    public void UpdateMP(float _maxMP, float _currentMP)
    {
        mpText.SetText("{0:0}/{1:0}", _currentMP, _maxMP);
        mpSlider.value = (_currentMP / _maxMP);
    }
    #endregion
}