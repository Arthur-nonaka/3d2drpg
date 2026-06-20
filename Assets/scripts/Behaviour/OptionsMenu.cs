using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance { get; private set; }

    [Header("UI References")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Transform panelsContainer;
    public RectTransform selectionDot;
    public AudioClip changeSound;

    public float dotDistance = -30f;

    private Stack<List<IAction>> actionStack = new();
    private List<RectTransform> currentButtons = new();
    private List<RectTransform> layerPanels = new();
    private int selectedIndex = 0;
    private int oldIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ChangeFocus(Transform position)
    {
        panelsContainer.position = position.position + new Vector3(4f, 2f, 0f);
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveSelection(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveSelection(1);
        else if (Input.GetKeyDown(KeyCode.Z))
            SelectCurrent(true);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SelectCurrent(false);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.X))
            PopLayer();
        else
            return;
    }

    public void ShowOptions(List<IAction> rootActions)
    {
        actionStack.Clear();
        PushLayer(rootActions);
        gameObject.SetActive(true);
    }

    private void PushLayer(List<IAction> actions)
    {
        actionStack.Push(actions);
        oldIndex = selectedIndex;
        selectedIndex = 0;
        RebuildUI();
        AnimateLayerTransition();
    }

    private void PopLayer()
    {
        if (actionStack.Count > 1)
        {
            actionStack.Pop();
            selectedIndex = oldIndex;
            RebuildUI();
            var panel = layerPanels[0];
            panel.localScale = new Vector3(0.8f, 0.8f, 1f);
            panel.anchoredPosition = new Vector2(
                panel.rect.width - (panel.rect.width * 0.1f),
                panel.anchoredPosition.y
            );
            AnimateLayerBackTransition();
        }
    }

    private void RebuildUI()
    {
        foreach (Transform child in panelsContainer)
            Destroy(child.gameObject);

        currentButtons.Clear();
        layerPanels.Clear();

        var currentLayer = actionStack.Peek();

        foreach (var layer in actionStack)
        {
            var panelObj = Instantiate(buttonContainer, panelsContainer);
            var panel = panelObj.GetComponent<RectTransform>();
            bool isCurrentLayer = layer == currentLayer;

            foreach (var action in layer)
            {
                var btnObj = Instantiate(buttonPrefab, panel.transform);
                var text = btnObj.GetComponentInChildren<TMPro.TMP_Text>();
                var icon = btnObj.transform.Find("Icon")?.GetComponentInChildren<Image>();
                var button = btnObj.GetComponent<Button>();

                text.text = action.Name;
                if (icon)
                    icon.sprite = action.Icon;

                if (button != null)
                {
                    button.interactable = isCurrentLayer && action.CanUse();

                    if (isCurrentLayer)
                        button.onClick.AddListener(() => OnActionSelected(action));
                }

                if (isCurrentLayer)
                    currentButtons.Add(btnObj.GetComponent<RectTransform>());
            }
            layerPanels.Add(panel);

            // if (!isCurrentLayer)
            // {
            //     panel.localScale = new Vector3(0.95f, 0.95f, 1f);
            //     panel.anchoredPosition = new Vector2(-180f, 0f);
            // }
            // else
            // {
            //     panel.localScale = Vector3.one;
            //     panel.anchoredPosition = Vector2.zero;
            // }
        }
        selectedIndex = Mathf.Clamp(selectedIndex, 0, currentButtons.Count - 1);
        UpdateSelectionIndicator();
    }

    private void MoveSelection(int direction)
    {
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, currentButtons.Count - 1);
        UpdateSelectionIndicator();
    }

    private void AnimateLayerTransition()
    {
        if (layerPanels.Count == 0 || layerPanels.Count == 1)
            return;
        for (int i = 0; i < layerPanels.Count; i++)
        {
            var panel = layerPanels[i];
            bool isCurrent = i == layerPanels.Count - 1;
            Vector3 targetScale = !isCurrent ? Vector3.one : new Vector3(0.8f, 0.8f, 1f);
            Vector2 targetPos = !isCurrent
                ? new Vector2(panel.anchoredPosition.x, panel.anchoredPosition.y)
                : new Vector2(
                    panel.rect.width - (panel.rect.width * 0.1f),
                    panel.anchoredPosition.y
                );
            AnimatePanel(panel, targetScale, targetPos, 0.10f);
        }
    }

    private void AnimateLayerBackTransition()
    {
        for (int i = 0; i < layerPanels.Count; i++)
        {
            var panel = layerPanels[i];
            Vector3 targetScale = Vector3.one;
            Vector2 targetPos = new Vector2(0f, panel.anchoredPosition.y);
            AnimatePanel(panel, targetScale, targetPos, 0.2f);
        }
    }

    private void AnimatePanel(
        RectTransform panel,
        Vector3 targetScale,
        Vector2 targetPos,
        float duration
    )
    {
        panel.DOKill();

        panel.DOScale(targetScale, duration).SetEase(Ease.InOutSine);

        panel.DOAnchorPos(targetPos, duration).SetEase(Ease.InOutSine);
    }

    private void UpdateSelectionIndicator()
    {
        if (selectionDot != null && currentButtons.Count > 0)
        {
            selectionDot.SetParent(currentButtons[selectedIndex], false);
            selectionDot.anchoredPosition = new Vector2(dotDistance, 0);
            selectionDot.gameObject.SetActive(true);
            SoundManager.Instance.PlaySettings(changeSound);
        }
    }

    private void SelectCurrent(bool isAction)
    {
        if (currentButtons.Count == 0)
            return;

        var action = actionStack.Peek()[selectedIndex];

        if (action is ActionCategory category)
        {
            PushLayer(category.GetActions());
        }
        else
        {
            if (isAction)
            {
                action.Execute();
                Hide();
            }
        }
    }

    private void OnActionSelected(IAction action)
    {
        if (action is ActionCategory category)
        {
            PushLayer(category.GetActions());
        }
        else
        {
            action.Execute();
            Hide();
        }
    }

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
