using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public SaveData CurrentData { get; private set; }
    public PlayerStats newPlayerStats;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        SaveManager.Save(CurrentData);
    }

    public void LoadGame()
    {
        CurrentData = SaveManager.Load();
        if (CurrentData == null)
            return;
    }

    public bool HasSaveData() => SaveManager.Load() != null;

    public void NewGame()
    {
        CurrentData = SaveManager.CreateNew(newPlayerStats);
    }

    public void UpdateFromCharacter(List<Character> characters)
    {
        List<CharacterSaveData> newSaveCharacters = new List<CharacterSaveData>();
        foreach (var c in characters)
        {
            newSaveCharacters.Add(
                new CharacterSaveData
                {
                    name = c.Name,
                    level = c.level,
                    currentXP = c.Experience,
                    HP = c.HP,
                    maxHP = c.MaxHP,
                    mana = c.Energy,
                    attack = c.Attack,
                    defense = c.Defense,
                    specialAttack = c.SpecialAttack,
                    specialDefense = c.SpecialDefense,
                    speed = c.Speed,
                    unlockedActionNames = new List<string>(),
                }
            );
        }
        CurrentData.Characters = newSaveCharacters;
    }
}
