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

    [Header("Order UI")]
    [SerializeField]
    private GameObject orderUIContainer;

    [SerializeField]
    private GameObject orderUIPrefab;

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

    public void UpdateOrderUI(Character[] turnOrder)
    {
        foreach (Transform child in orderUIContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var character in turnOrder)
        {
            var orderUI = Instantiate(orderUIPrefab, orderUIContainer.transform);
            var images = orderUI.GetComponentsInChildren<Image>();
            images[0].color = character.IsPlayerControlled
                ? new Color(0, 0, 1, 0.2f)
                : new Color(1, 0, 0, 0.2f);
            images[1].preserveAspect = true;
            var view = BattleManager.Instance.characterViews.Find(v => v.name == character.Name);
            if (view != null)
                images[1].sprite = view.DefaultSprite;
        }
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
        // switch (type) {
        // case ButtonType.Attack:
        //     BattleManager.Instance.PlayerChoseAttack();
        //     break;
        //     case ButtonType.Defend:
        //         BattleManager.Instance.PlayerChoseDefend();
        //         break;
        //     case ButtonType.UseItem:
        //         BattleManager.Instance.PlayerChoseUseItem();
        //         break;
        // }
        SetButtonInteractable(false);
    }

    public void UpdateSpecialBar(float value)
    {
        specialBarImage.fillAmount = Mathf.Clamp01(value);
    }
}
