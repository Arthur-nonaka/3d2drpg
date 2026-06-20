using System;
using UnityEngine;

public enum ItemEffectType
{
    Heal,
    RestoreEnergy,
    Revive,
    Damage,
    DamageBoost,
    DefenseBoost,
}

public enum ItemTargetType
{
    Self,
    Ally,
    Enemy,
    AllAllies,
    AllEnemies,
}

[CreateAssetMenu(menuName = "Options/Battle/Item")]
public class ItemAction : ScriptableObject, IAction
{
    [SerializeField]
    private string actionName;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private ItemEffectType effectType;

    [SerializeField]
    public ItemTargetType targetType;

    [SerializeField]
    private int amount;

    [SerializeField]
    private int turnsDuration;

    [SerializeField]
    private int quantity;

    public string Name => actionName;
    public Sprite Icon => icon;
    public int ManaCost => 0;
    public bool IsSpecial => false;
    public int RequiredLevel => 1;

    public bool CanUse() => quantity > 0;

    public void Execute()
    {
        if (!CanUse())
            return;
        BattleManager.Instance.PlayerChoseItem(this);
    }

    public (int, bool) Apply(Character target)
    {
        if (quantity <= 0)
            return (0, false);

        switch (effectType)
        {
            case ItemEffectType.Heal:
                return target.Heal(amount);
            case ItemEffectType.RestoreEnergy:
                target.Energy += amount;
                return (0, false);
            // case ItemEffectType.Revive:
            //     if (target.IsDead)
            //         target.Revive(amount);
            //     break;
            case ItemEffectType.Damage:
                target.TakeDamage(amount);
                return (amount, true);
            case ItemEffectType.DamageBoost:
                target.AddStatusEffect(new BoostDamageStatusEffect(amount, turnsDuration));
                return (0, false);
            // case ItemEffectType.DefenseBoost:
            //     target.DefenseBoost(amount);
            //     break;
            default:
                Debug.LogWarning("Unknown item effect type: " + effectType);
                return (0, false);
        }
    }
}
