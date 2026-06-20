using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Character
{
    public string Name { get; set; }
    public bool IsPlayerControlled { get; set; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int Energy { get; set; }
    public int Speed { get; set; }
    public int Defense { get; set; }
    public int SpecialDefense { get; set; }
    public int Attack { get; set; }
    public int SpecialAttack { get; set; }
    public int Luck { get; set; }

    public float CritChance => Luck * 0.005f;
    public float CritMultiplier => 2f;

    public int level = 1;
    public int Experience { get; set; }
    public int ExperienceToNextLevel =>
        LevelFormula.XPForLevel(level + 1) - LevelFormula.XPForLevel(level);

    public List<IAction> Actions { get; set; } = new List<IAction>();

    public int DamageBoost { get; private set; }
    public List<IStatusEffect> ActiveEffects { get; private set; } = new List<IStatusEffect>();
    public bool IsDead => HP <= 0;

    public Action OnDeath;

    public Character(
        string name,
        bool isPlayerControlled,
        int maxHP,
        int hp,
        int energy,
        int speed,
        int defense,
        int specialDefense,
        int attack,
        int specialAttack,
        int luck = 5,
        int level = 1,
        int experience = 0,
        List<IAction> actions = null
    )
    {
        Name = name;
        IsPlayerControlled = isPlayerControlled;
        MaxHP = maxHP;
        HP = hp;
        Energy = energy;
        Speed = speed;
        Defense = defense;
        SpecialDefense = specialDefense;
        Attack = attack;
        SpecialAttack = specialAttack;
        Luck = luck;
        this.level = level;
        Experience = experience;
        Actions = actions ?? new List<IAction>();
    }

    public void AddStatusEffect(IStatusEffect effect)
    {
        if (effect == null)
            return;
        ActiveEffects.Add(effect);
        effect.OnApply(this);
    }

    public void RemoveStatusEffect(IStatusEffect effect)
    {
        if (effect == null)
            return;
        if (ActiveEffects.Remove(effect))
            effect.OnRemove(this);
    }

    public void ModifyDamageBoost(int amount)
    {
        DamageBoost += amount;
        if (DamageBoost < 0)
            DamageBoost = 0;
    }

    public void ProcessStartOfTurnEffects()
    {
        for (int i = ActiveEffects.Count - 1; i >= 0; i--)
        {
            var effect = ActiveEffects[i];
            effect.OnTurnStart(this);
            if (IsDead)
                break;
        }
    }

    public void ProcessEndOfTurnEffects()
    {
        for (int i = ActiveEffects.Count - 1; i >= 0; i--)
        {
            var effect = ActiveEffects[i];
            effect.OnTurnEnd(this);
            effect.Tick();
            if (effect.IsExpired)
            {
                effect.OnRemove(this);
                ActiveEffects.RemoveAt(i);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP < 0)
            HP = 0;
        if (HP == 0)
        {
            OnDeath?.Invoke();
        }

        NotifyHealthChanged();
    }

    public (int, bool) Heal(int amount)
    {
        HP += amount;
        int hpRestored = amount;
        if (HP > MaxHP)
        {
            hpRestored = MaxHP - (HP - amount);
            HP = MaxHP;
        }
        NotifyHealthChanged();
        return (hpRestored, true);
    }

    public event Action<int, int> OnHealthChanged;

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(HP, MaxHP);
    }

    public void AddXp(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        Experience = 0;
        MaxHP += LevelUpStatsGain.HealthGain;
        HP = MaxHP;
        Attack += LevelUpStatsGain.AttackGain;
        Defense += LevelUpStatsGain.DefenseGain;
        SpecialAttack += LevelUpStatsGain.SpecialAttackGain;
        SpecialDefense += LevelUpStatsGain.SpecialDefenseGain;
        Speed += LevelUpStatsGain.SpeedGain;
        Luck += LevelUpStatsGain.LuckGain;
    }
}
