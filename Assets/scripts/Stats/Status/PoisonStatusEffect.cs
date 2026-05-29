using System;

public class PoisonStatusEffect : IStatusEffect
{
    public string Name { get; private set; } = "Poison";
    public int RemainingTurns { get; private set; }
    private int damagePerTurn;

    public bool IsExpired => RemainingTurns <= 0;

    public PoisonStatusEffect(int damagePerTurn, int duration)
    {
        this.damagePerTurn = damagePerTurn;
        this.RemainingTurns = duration;
    }

    public void OnApply(Character target)
    {
        // could show UI/log here
    }

    public void OnTurnStart(Character target)
    {
        if (target == null)
            return;
        target.TakeDamage(damagePerTurn);
    }

    public void OnTurnEnd(Character target)
    {
        // nothing special at end
    }

    public void OnRemove(Character target)
    {
        // cleanup if needed
    }

    public void Tick()
    {
        RemainingTurns--;
    }
}
