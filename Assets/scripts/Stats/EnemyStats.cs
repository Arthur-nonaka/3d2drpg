using UnityEngine;

[CreateAssetMenu(menuName = "Stats/EnemyStats", fileName = "EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public string Name;
    public int Health;
    public int Mana;
    public int Attack;
    public int SpecialAttack;
    public int SpecialDefense;
    public int Defense;
    public int Speed;
    public int Luck = 5;
    public int XPReward;
    public GameObject prefab;
}
