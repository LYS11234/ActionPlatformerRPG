using UnityEngine;

[System.Serializable]
public class Parameters
{
    [field:SerializeField]
    public float CurrentHP {  get; private set; }
    private float baseMaxHP;
    [field: SerializeField]
    public float MaxHP { get; private set; }
    private float additionalMaxHP;
    [field: SerializeField]
    public float CurrentMP { get; private set; }
    [field: SerializeField]
    public float MaxMP { get; private set; }

    public void Init(float _maxHP, float _maxMP)
    {
        additionalMaxHP = 1f;
        baseMaxHP = _maxHP;
        MaxHP = _maxHP;
        CurrentHP = _maxHP;
        CurrentMP = _maxMP;
        MaxMP = _maxMP;
    }

    public void UpdateCurrentHP(float _value)
    {
        CurrentHP += _value;
        CurrentHP = Mathf.Clamp(CurrentHP, 0f, MaxHP);
    }
    
    public void UpdateMaxHP(int _value)
    {
        baseMaxHP += _value;
        UpdateHP();
    }

    public void UpdateMaxHP(float _value)
    {
        if(MaxHP <= 0)
        {
            Debug.LogError("MAX HP IS UNDER 0!");
            return;
        }
        additionalMaxHP += _value;
        UpdateHP();
    }


    private void UpdateHP()
    {
        float ratio = CurrentHP / MaxHP;

        MaxHP = baseMaxHP * additionalMaxHP;
        CurrentHP = MaxHP * ratio;
        CurrentHP = Mathf.Clamp(CurrentHP, 0f, MaxHP);
    }
}
