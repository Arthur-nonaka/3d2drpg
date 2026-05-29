using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string SavePath(string slot) =>
        Application.persistentDataPath + $"/saves/{slot}.json";

    public static void Save(SaveData data, string slot = "save")
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath(slot), json);
    }

    public static SaveData Load(string slot = "save")
    {
        string path = SavePath(slot);
        if (!File.Exists(path))
            return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static SaveData CreateNew(PlayerStats playerStats)
    {
        return new SaveData
        {
            Characters = new List<CharacterSaveData>()
            {
                new CharacterSaveData
                {
                    name = playerStats.Name,
                    level = 1,
                    currentXP = 0,
                    HP = playerStats.Health,
                    maxHP = playerStats.Health,
                    mana = playerStats.Mana,
                    attack = playerStats.Attack,
                    defense = playerStats.Defense,
                    specialAttack = playerStats.SpecialAttack,
                    specialDefense = playerStats.SpecialDefense,
                    speed = playerStats.Speed,
                    unlockedActionNames = new List<string>(),
                },
            },
            SaveVersion = "1.0",
            LastSaveTime = System.DateTime.Now.ToString(),
        };
    }
}
