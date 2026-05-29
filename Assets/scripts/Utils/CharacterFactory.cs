public static class CharacterFactory
{
    public static Character CreateFromPlayerStats(PlayerStats playerStats)
    {
        return new Character(
            name: playerStats.Name,
            isPlayerControlled: true,
            maxHP: playerStats.Health,
            hp: playerStats.Health,
            energy: playerStats.Mana,
            speed: playerStats.Speed,
            defense: playerStats.Defense,
            specialDefense: playerStats.SpecialDefense,
            attack: playerStats.Attack,
            specialAttack: playerStats.SpecialAttack,
            actions: playerStats.Actions,
            level: playerStats.Level,
            experience: playerStats.Experience
        );
    }

    public static Character CreateFromSaveData(CharacterSaveData saveData)
    {
        return new Character(
            name: saveData.name,
            isPlayerControlled: true,
            maxHP: saveData.maxHP,
            hp: saveData.HP,
            energy: saveData.mana,
            speed: saveData.speed,
            defense: saveData.defense,
            specialDefense: saveData.specialDefense,
            attack: saveData.attack,
            specialAttack: saveData.specialAttack,
            level: saveData.level,
            experience: saveData.currentXP
        );
    }
}
