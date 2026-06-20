using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<CharacterSaveData> Characters;
    public string SaveVersion = "1.0";
    public string LastSaveTime;
}

[Serializable]
public class CharacterSaveData
{
    public string name;
    public int level;
    public int currentXP;
    public int HP;
    public int maxHP;
    public int mana;
    public LevelUpStatGain levelUpStats;
    public int attack,
        defense,
        specialAttack,
        specialDefense,
        speed,
        luck;
    public List<string> unlockedActionNames;
}
