using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private TextMeshProUGUI ExpText;
    private Character playerStats;

    public void Initialize(Character character)
    {
        playerStats = character;
        playerStats.OnHealthChanged += UpdateHealthText;
        ExpText.text =
            $"Level {playerStats.level} - {playerStats.Experience} / {playerStats.ExperienceToNextLevel} XP";
        UpdateHealthText(playerStats.HP, playerStats.MaxHP);
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnHealthChanged -= UpdateHealthText;
    }

    private void UpdateHealthText(int current, int max)
    {
        healthText.text = $"{current} / {max} Life";
    }
}
