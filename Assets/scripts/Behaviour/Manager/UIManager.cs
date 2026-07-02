using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    [SerializeField]
    private GameObject SelectedImage;

    [SerializeField]
    private float slotStep = 140f;

    [SerializeField]
    private float slideDuration = 0.3f;

    private Queue<GameObject> freeSlots = new Queue<GameObject>();
    private bool layoutDisabled;
    private Character[] currentOrder;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        var rt = SelectedImage.GetComponent<RectTransform>();
        int pixelSteps = 3;
        float stepDuration = 0.3f;
        float interCycleDelay = 1.3f;

        Sequence seq = DOTween.Sequence();
        seq.Append(
            DOVirtual.Float(
                0,
                pixelSteps,
                stepDuration,
                v =>
                {
                    float t = Mathf.Floor(v) / pixelSteps;
                    rt.localScale = Vector3.one * Mathf.Lerp(1f, 0.8f, t);
                }
            )
        );
        seq.Append(
            DOVirtual.Float(
                pixelSteps,
                0,
                stepDuration,
                v =>
                {
                    float t = Mathf.Floor(v) / pixelSteps;
                    rt.localScale = Vector3.one * Mathf.Lerp(1f, 0.8f, t);
                }
            )
        );
        seq.AppendInterval(interCycleDelay);
        seq.SetLoops(-1, LoopType.Restart);
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
        currentOrder = turnOrder;

        if (freeSlots.Count < 1)
        {
            foreach (var c in turnOrder)
                freeSlots.Enqueue(InstantiateOrder(c));
            PositionAll(0f);
            UpdateSlotVisuals(turnOrder);
            return;
        }

        float rightmostX = freeSlots.Last().GetComponent<RectTransform>().anchoredPosition.x;

        var exitSlot = freeSlots.Dequeue();
        var exitRt = exitSlot.GetComponent<RectTransform>();
        exitRt.DOKill();
        exitRt
            .DOAnchorPosX(exitRt.anchoredPosition.x - slotStep, slideDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(exitSlot));

        foreach (var slot in freeSlots)
        {
            var rt = slot.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOAnchorPosX(rt.anchoredPosition.x - slotStep, slideDuration).SetEase(Ease.Linear);
        }

        var newSlot = InstantiateOrder(turnOrder[turnOrder.Length - 1]);
        var newRt = newSlot.GetComponent<RectTransform>();
        newRt.anchoredPosition = new Vector2(rightmostX + slotStep, newRt.anchoredPosition.y);
        newRt.DOAnchorPosX(rightmostX, slideDuration).SetEase(Ease.Linear);
        freeSlots.Enqueue(newSlot);

        UpdateSlotVisuals(turnOrder);
    }

    public void RebuildOrderUI(Character[] turnOrder)
    {
        currentOrder = turnOrder;

        foreach (var slot in freeSlots)
            Destroy(slot);
        freeSlots.Clear();

        foreach (var c in turnOrder)
            freeSlots.Enqueue(InstantiateOrder(c));
        PositionAll(0f);
        UpdateSlotVisuals(turnOrder);
    }

    public void RefreshSlotVisuals(Character[] turnOrder)
    {
        currentOrder = turnOrder;
        UpdateSlotVisuals(turnOrder);
    }

    private void UpdateSlotVisuals(Character[] turnOrder)
    {
        var slots = freeSlots.ToArray();
        for (int i = 0; i < slots.Length && i < turnOrder.Length; i++)
        {
            var character = turnOrder[i];
            var images = slots[i].GetComponentsInChildren<Image>();
            if (character.IsDead && character.IsPlayerControlled)
            {
                images[0].color = new Color(0.3f, 0.3f, 0.3f, 0.35f);
                images[1].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            else
            {
                images[0].color = character.IsPlayerControlled
                    ? new Color(0, 0, 1, 0.35f)
                    : new Color(1, 0, 0, 0.35f);
                images[1].color = Color.white;
            }
        }
    }

    private void PositionAll(float duration)
    {
        int count = freeSlots.Count;
        float totalWidth = (count - 1) * slotStep;
        float startX = -totalWidth / 2f;

        var slots = freeSlots.ToArray();
        for (int i = 0; i < count; i++)
        {
            var rt = slots[i].GetComponent<RectTransform>();
            float targetX = startX + i * slotStep;
            if (duration > 0f)
                rt.DOAnchorPosX(targetX, duration).SetEase(Ease.Linear);
            else
                rt.anchoredPosition = new Vector2(targetX, rt.anchoredPosition.y);
        }
    }

    GameObject InstantiateOrder(Character character)
    {
        GameObject order = Instantiate(orderUIPrefab, orderUIContainer.transform);
        var images = order.GetComponentsInChildren<Image>();
        if (character.IsDead && character.IsPlayerControlled)
        {
            images[0].color = new Color(0.3f, 0.3f, 0.3f, 0.35f);
            images[1].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            images[0].color = character.IsPlayerControlled
                ? new Color(0, 0, 1, 0.35f)
                : new Color(1, 0, 0, 0.35f);
        }
        images[1].preserveAspect = true;
        var view = BattleManager.Instance.characterViews.Find(v => v.name == character.Name);
        if (view != null)
            images[1].sprite = view.DefaultSprite;
        return order;
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
        SetButtonInteractable(false);
    }

    public void UpdateSpecialBar(float value)
    {
        specialBarImage.fillAmount = Mathf.Clamp01(value);
    }
}
