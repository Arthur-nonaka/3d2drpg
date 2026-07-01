using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Options/Battle/Offense/Attack")]
public class AttackAction : ScriptableObject, IAction
{
    [SerializeField]
    private string actionName;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private int baseDamage;

    [SerializeField]
    private float multiplier;

    [SerializeField]
    private int manaCost;

    [SerializeField]
    private bool isSpecial;

    public string Name => actionName;
    public Sprite Icon => icon;
    public int ManaCost => manaCost;
    public int BaseDamage => baseDamage;
    public float Multiplier => multiplier;
    public bool IsSpecial => isSpecial;
    public int RequiredLevel => 1;

    public bool CanUse()
    {
        if (ManaCost > 0)
        {
            if (IsSpecial)
            {
                if (SpecialBarManager.Instance.CanUse(ManaCost))
                {
                    return true;
                }
            }
            else
            {
                if (BattleManager.Instance.GetCurrentCharacter().Energy >= ManaCost)
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            return true;
        }
    }

    public void Execute()
    {
        if (!CanUse())
            return;

        var character = BattleManager.Instance.GetCurrentCharacter();
        var damage = Mathf.RoundToInt(baseDamage * multiplier);
        damage += character.DamageBoost;

        if (IsSpecial)
        {
            SpecialBarManager.Instance.UseCharge(ManaCost);
        }
        else
        {
            character.Energy -= ManaCost;
            SpecialBarManager.Instance.AddCharge(10);
            if (character.Energy < 0)
                character.Energy = 0;
        }

        BattleManager.Instance.StartTargeting(damage);
    }
}
