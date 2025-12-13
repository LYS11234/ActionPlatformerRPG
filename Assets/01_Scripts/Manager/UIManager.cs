using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField]
    private float duration = 1f;
    [SerializeField]
    private Ease easeType;

    public void UpdateBullet(int _currentBullets)
    {
        bulletText.SetText("× {0:00}", _currentBullets);
    }
    #region HP UI

    public void UpdateHP(float _maxHP, float _currentHP)
    {
        hpText.SetText("{0:0}/{1:0}", _currentHP, _maxHP);
        float _targetRatio = _currentHP / _maxHP;
        

        hpSlider.DOKill();
        hpSlider.DOValue(_targetRatio, duration).SetEase(easeType);
    }

    
    #endregion

    #region MP UI
    public void UpdateMP(float _maxMP, float _currentMP)
    {
        mpText.SetText("{0:0}/{1:0}", _currentMP, _maxMP);
        float _targetRatio = _currentMP / _maxMP;

        mpSlider.DOKill();
        mpSlider.DOValue(_targetRatio, duration).SetEase(easeType);
    }
    #endregion
}