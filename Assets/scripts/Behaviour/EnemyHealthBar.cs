using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image FillSprite;
    public Transform fillBar;

    public void UpdateHealthBar(float fillAmount)
    {
        FillSprite.fillAmount = fillAmount;
        // fillBar.localScale = new Vector3(fillAmount, 1f, 1f);
    }
}
