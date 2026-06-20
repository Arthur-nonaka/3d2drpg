using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/PlayerStats", fileName = "PlayerStats")]
public class PlayerStats : ScriptableObject
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
    public int Level;
    public int Experience;
    public float Velocity = 5f;
    public GameObject prefab;

    [SerializeField]
    private List<ScriptableObject> actionObjects;
    public List<IAction> Actions
    {
        get
        {
            var list = new List<IAction>();
            foreach (var obj in actionObjects)
            {
                if (obj is IAction action)
                    list.Add(action);
            }
            return list;
        }
    }
}
