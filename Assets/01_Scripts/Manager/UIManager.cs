using System.Collections;
using System.Threading.Tasks;
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

    private Coroutine hpCoroutine;

    private float hpCurrentTime;

    public void UpdateBullet(int _currentBullets)
    {
        bulletText.SetText("× {0:00}", _currentBullets);
    }
    #region HP UI

    public void UpdateHP(float _maxHP, float _currentHP)
    {
        hpText.SetText("{0:0}/{1:0}", _currentHP, _maxHP);
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }
        hpCoroutine = StartCoroutine(HPAnimation(_currentHP/_maxHP));
    }

    private IEnumerator HPAnimation(float currentHPRatio)
    {
        float value = hpSlider.value;
        hpCurrentTime = 0f;
        while (hpCurrentTime < 1f)
        {
            
            hpSlider.value = Mathf.Lerp(value, currentHPRatio, hpCurrentTime);
            hpCurrentTime += Time.deltaTime;
            yield return null;
        }
        hpSlider.value = currentHPRatio;
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