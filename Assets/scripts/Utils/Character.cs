using System;
using System.Collections.Generic;

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

    public int level = 1;
    public int Experience { get; set; }
    public int ExperienceToNextLevel =>
        LevelFormula.XPForLevel(level + 1) - LevelFormula.XPForLevel(level);

    public List<IAction> Actions { get; set; } = new List<IAction>();

    public int DamageBoost { get; private set; }
    public List<IStatusEffect> ActiveEffects { get; private set; } = new List<IStatusEffect>();
    public bool IsDead => HP <= 0;

    public LevelUpStatGain LevelUpStats { get; set; }

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
        NotifyHealthChanged();
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > MaxHP)
            HP = MaxHP;
        NotifyHealthChanged();
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
        if (LevelUpStats != null)
        {
            MaxHP += LevelUpStats.HP;
            HP = MaxHP;
            Attack += LevelUpStats.Attack;
            Defense += LevelUpStats.Defense;
            SpecialAttack += LevelUpStats.SpecialAttack;
            SpecialDefense += LevelUpStats.SpecialDefense;
            Speed += LevelUpStats.Speed;
        }
    }
}
