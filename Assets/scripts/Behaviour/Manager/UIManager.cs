using System;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    Attack,
    Defend,
    UseItem,
}

public class UIManager : MonoBehaviour
{
    [Header("Battle UI")]
    [SerializeField]
    private Button attackButton;

    [SerializeField]
    private Button specialButton;

    [SerializeField]
    private Button useItemButton;

    [SerializeField]
    private Image specialBarImage;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetButtonInteractable(bool interactable)
    {
        if (attackButton != null)
            attackButton.interactable = interactable;
        if (specialButton != null)
            specialButton.interactable = interactable;
        if (useItemButton != null)
            useItemButton.interactable = interactable;
    }

    public void OnAttackButtonPressed()
    {
        OnButtonPressed(ButtonType.Attack);
    }

    public void OnDefendButtonPressed()
    {
        OnButtonPressed(ButtonType.Defend);
    }

    public void OnUseItemButtonPressed()
    {
        OnButtonPressed(ButtonType.UseItem);
    }

    public void OnButtonPressed(ButtonType type)
    {
        switch (type) {
            // case ButtonType.Attack:
            //     BattleManager.Instance.PlayerChoseAttack();
            //     break;
            //     case ButtonType.Defend:
            //         BattleManager.Instance.PlayerChoseDefend();
            //         break;
            //     case ButtonType.UseItem:
            //         BattleManager.Instance.PlayerChoseUseItem();
            //         break;
        }
        SetButtonInteractable(false);
    }

    public void UpdateSpecialBar(float value)
    {
        specialBarImage.fillAmount = Mathf.Clamp01(value);
    }
}
