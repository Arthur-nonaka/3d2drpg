using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/LevelUpStatGain", fileName = "LevelUpStatGain")]
public class LevelUpStatGain : ScriptableObject
{
    public string Name;
    public int HP;
    public int Mana;
    public int Attack;
    public int SpecialAttack;
    public int Defense;
    public int SpecialDefense;
    public int Speed;
}
