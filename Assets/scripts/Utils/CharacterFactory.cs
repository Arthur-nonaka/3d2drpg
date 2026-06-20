public static class CharacterFactory
{
    public static Character CreateFromPlayerStats(PlayerStats playerStats)
    {
        var character = new Character(
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
            luck: playerStats.Luck,
            actions: playerStats.Actions,
            level: playerStats.Level,
            experience: playerStats.Experience
        );

        return character;
    }

    public static Character CreateFromEnemyStats(EnemyStats enemyStats)
    {
        return new Character(
            name: enemyStats.Name,
            isPlayerControlled: false,
            maxHP: enemyStats.Health,
            hp: enemyStats.Health,
            energy: enemyStats.Mana,
            speed: enemyStats.Speed,
            defense: enemyStats.Defense,
            specialDefense: enemyStats.SpecialDefense,
            attack: enemyStats.Attack,
            specialAttack: enemyStats.SpecialAttack,
            luck: enemyStats.Luck,
            level: 1,
            experience: enemyStats.XPReward
        );
    }

    public static Character CreateFromSaveData(CharacterSaveData saveData)
    {
        var character = new Character(
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
            luck: saveData.luck,
            level: saveData.level,
            experience: saveData.currentXP
        );

        return character;
    }
}
