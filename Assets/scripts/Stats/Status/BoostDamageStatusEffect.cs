using System;

public class BoostDamageStatusEffect : IStatusEffect
{
    public string Name { get; private set; } = "Boost Damage";
    public int RemainingTurns { get; private set; }
    private int damageBoost;

    public bool IsExpired => RemainingTurns <= 0;

    public BoostDamageStatusEffect(int damageBoost, int duration)
    {
        this.damageBoost = damageBoost;
        this.RemainingTurns = duration;
    }

    public void OnApply(Character target)
    {
        // apply the boost immediately when the effect starts
        target.ModifyDamageBoost(damageBoost);
    }

    public void OnTurnStart(Character target) { }

    public void OnTurnEnd(Character target)
    {
        // nothing special at end
    }

    public void OnRemove(Character target)
    {
        // remove the boost when the effect expires
        target.ModifyDamageBoost(-damageBoost);
    }

    public void Tick()
    {
        RemainingTurns--;
    }
}
