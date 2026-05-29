using System;

public interface IStatusEffect
{
    string Name { get; }
    int RemainingTurns { get; }
    bool IsExpired { get; }

    void OnApply(Character target);
    void OnTurnStart(Character target);
    void OnTurnEnd(Character target);
    void OnRemove(Character target);

    // Decrement duration (called once per turn end)
    void Tick();
}
