using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private TextMeshProUGUI ExpText;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private Image delayedFill;

    [SerializeField]
    private Image expBarFill;

    [SerializeField]
    private float smoothDuration = 0.3f;

    [SerializeField]
    private float delayedDuration = 0.6f;

    [SerializeField]
    private float delayBeforeDrain = 0.5f;

    private Character playerStats;
    private int lastDisplayedHP;

    public void Initialize(Character character)
    {
        playerStats = character;
        playerStats.OnHealthChanged += UpdateHealthDisplay;
        lastDisplayedHP = playerStats.HP;
        UpdateHealthDisplay(playerStats.HP, playerStats.MaxHP);
        UpdateExpDisplay();
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnHealthChanged -= UpdateHealthDisplay;
    }

    private void UpdateHealthDisplay(int current, int max)
    {
        float fill = (float)current / max;

        healthBarFill.DOKill();
        if (delayedFill != null)
            delayedFill.DOKill();
        DOTween.Kill("hpTextTween");

        Color targetColor = GetHealthColor(fill);

        healthBarFill.color = Color.white;
        healthBarFill.DOColor(targetColor, 0.25f).SetEase(Ease.OutCubic);

        healthBarFill.DOFillAmount(fill, smoothDuration).SetEase(Ease.OutCubic);

        if (delayedFill != null)
        {
            delayedFill
                .DOFillAmount(fill, delayedDuration)
                .SetDelay(delayBeforeDrain)
                .SetEase(Ease.OutCubic);
        }

        int startHP = lastDisplayedHP;
        lastDisplayedHP = current;
        DOVirtual
            .Int(
                startHP,
                current,
                smoothDuration,
                v =>
                {
                    healthText.text = $"{v} / {max} Life";
                }
            )
            .SetId("hpTextTween")
            .SetEase(Ease.OutCubic);
    }

    public void UpdateExpDisplay()
    {
        if (playerStats == null)
            return;

        if (expBarFill != null)
        {
            float fill = (float)playerStats.Experience / playerStats.ExperienceToNextLevel;
            expBarFill.DOFillAmount(fill, 0.3f).SetEase(Ease.OutCubic);
        }

        ExpText.text =
            $"Level {playerStats.level} - {playerStats.Experience} / {playerStats.ExperienceToNextLevel} XP";
    }

    private Color GetHealthColor(float fill)
    {
        if (fill > 0.6f)
            return Color.Lerp(Color.yellow, new Color(0.2f, 0.8f, 0.2f), (fill - 0.6f) / 0.4f);
        if (fill > 0.3f)
            return Color.Lerp(new Color(0.9f, 0.5f, 0f), Color.yellow, (fill - 0.3f) / 0.3f);
        return Color.Lerp(new Color(0.8f, 0.1f, 0.1f), new Color(0.9f, 0.5f, 0f), fill / 0.3f);
    }
}
