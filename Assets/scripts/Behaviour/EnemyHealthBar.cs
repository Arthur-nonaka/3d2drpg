using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject healthBarPrefab;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 2.5f, 0);

    [SerializeField]
    private Vector2 barSize = new Vector2(80, 6);

    [SerializeField]
    private float smoothDuration = 0.3f;

    [SerializeField]
    private float delayedDuration = 0.6f;

    [SerializeField]
    private float delayBeforeDrain = 0.5f;

    private Transform target;
    private Image fillSprite;
    private Image delayedFill;
    private Image background;
    private RectTransform fillRect;
    private RectTransform delayedRect;
    private RectTransform bgRect;
    private RectTransform canvasRect;
    private Camera cam;

    private void Awake()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in scene", this);
            return;
        }
        canvasRect = canvas.GetComponent<RectTransform>();
        cam = Camera.main;
        target = transform.parent;

        var instance = Instantiate(healthBarPrefab, canvasRect);
        instance.name = "EnemyHealthBar";
        instance.GetComponent<RectTransform>().sizeDelta = barSize;
        fillSprite = instance.transform.Find("Fill").GetComponent<Image>();
        fillRect = fillSprite.GetComponent<RectTransform>();
        delayedFill = instance.transform.Find("DelayedFill")?.GetComponent<Image>();
        delayedRect = delayedFill != null ? delayedFill.GetComponent<RectTransform>() : null;
        background = instance.transform.Find("Background").GetComponent<Image>();
        bgRect = background.GetComponent<RectTransform>();

        SetVisible(false);
    }

    private void LateUpdate()
    {
        if (target == null || canvasRect == null || fillRect == null)
            return;

        Vector2 screenPos = cam.WorldToScreenPoint(target.position + offset);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out anchoredPos
        );

        bgRect.anchoredPosition = anchoredPos;
        fillRect.anchoredPosition = anchoredPos;
        if (delayedRect != null)
            delayedRect.anchoredPosition = anchoredPos;
    }

    public void SetVisible(bool visible)
    {
        if (bgRect != null) bgRect.gameObject.SetActive(visible);
        if (fillRect != null) fillRect.gameObject.SetActive(visible);
        if (delayedRect != null) delayedRect.gameObject.SetActive(visible);
    }

    public void UpdateHealthBar(float fillAmount)
    {
        fillAmount = Mathf.Clamp01(fillAmount);

        fillSprite.DOKill();
        if (delayedFill != null)
            delayedFill.DOKill();

        Color targetColor = Color.red;

        fillSprite.color = Color.white;
        fillSprite.DOColor(targetColor, 0.25f).SetEase(Ease.OutCubic);

        fillSprite.DOFillAmount(fillAmount, smoothDuration).SetEase(Ease.OutCubic);

        if (delayedFill != null)
        {
            delayedFill
                .DOFillAmount(fillAmount, delayedDuration)
                .SetDelay(delayBeforeDrain)
                .SetEase(Ease.OutCubic);
        }
    }

    private Color GetHealthColor(float fill)
    {
        if (fill > 0.6f)
            return Color.Lerp(Color.yellow, new Color(0.2f, 0.8f, 0.2f), (fill - 0.6f) / 0.4f);
        if (fill > 0.3f)
            return Color.Lerp(new Color(0.9f, 0.5f, 0f), Color.yellow, (fill - 0.3f) / 0.3f);
        return Color.Lerp(new Color(0.8f, 0.1f, 0.1f), new Color(0.9f, 0.5f, 0f), fill / 0.3f);
    }

    private void OnDestroy()
    {
        if (bgRect != null)
            Destroy(bgRect.gameObject);
        if (fillRect != null)
            Destroy(fillRect.gameObject);
        if (delayedRect != null)
            Destroy(delayedRect.gameObject);
    }
}
