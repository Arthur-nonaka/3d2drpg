using UnityEngine;

public interface IAction
{
    string Name { get; }
    Sprite Icon { get; }
    int RequiredLevel { get; }
    int ManaCost
    {
        get => 0;
    }
    bool IsSpecial
    {
        get => false;
    }
    bool CanUse();
    void Execute();
}
