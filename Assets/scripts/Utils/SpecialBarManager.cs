using UnityEngine;

public class SpecialBarManager
{
    public static SpecialBarManager instance;
    public static SpecialBarManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SpecialBarManager();
            return instance;
        }
    }
    private float maxValue = 100f;
    private float currentValue = 0f;

    private SpecialBarManager() { }

    public float NormalizedValue => currentValue / maxValue;

    public void AddCharge(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0f, maxValue);
        UIManager.Instance.UpdateSpecialBar(NormalizedValue);
    }

    public bool CanUse(float cost) => currentValue >= cost;

    public void UseCharge(float cost)
    {
        if (!CanUse(cost))
            return;

        Debug.Log($"Using {cost} charge from Special Bar.");

        currentValue = Mathf.Clamp(currentValue - cost, 0f, maxValue);
        UIManager.Instance.UpdateSpecialBar(NormalizedValue);
    }

    public void ResetBar()
    {
        currentValue = 0f;
        UIManager.Instance.UpdateSpecialBar(0f);
    }
}
