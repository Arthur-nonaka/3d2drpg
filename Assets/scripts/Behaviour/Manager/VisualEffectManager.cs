using TMPro;
using UnityEngine;

public class VisualEffectManager : MonoBehaviour
{
    public static VisualEffectManager Instance { get; private set; }

    [SerializeField]
    private GameObject hitEffectPrefab;

    [SerializeField]
    private GameObject floatingTextPrefab;

    [SerializeField]
    private AudioClip attackSound;

    [SerializeField]
    private GameObject criticalHitEffectPrefab;

    [SerializeField]
    private AudioClip criticalAttackSound;

    [SerializeField]
    private AudioClip HealSound;

    [SerializeField]
    private RectTransform canvasTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (canvasTransform == null)
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas != null)
                canvasTransform = canvas.GetComponent<RectTransform>();
        }
    }

    public void PlayAttackEffect(
        Vector3 position,
        Vector3 sourcePosition,
        bool isCritical = false,
        HitEffectConfig config = null
    )
    {
        var prefab =
            config != null && config.prefabOverride != null
                ? config.prefabOverride
                : hitEffectPrefab;
        if (prefab != null)
            SpawnEffect(prefab, position, config);

        if (!isCritical)
            SoundManager.Instance.PlaySFX(attackSound);
        else
            SoundManager.Instance.PlaySFX(criticalAttackSound);
    }

    public void PlayCriticalAttackEffect(
        Vector3 position,
        Vector3 sourcePosition,
        HitEffectConfig config = null
    )
    {
        var prefab =
            config != null && config.prefabOverride != null ? config.prefabOverride
            : criticalHitEffectPrefab != null ? criticalHitEffectPrefab
            : hitEffectPrefab;
        if (prefab != null)
            SpawnEffect(prefab, position, config);

        if (criticalAttackSound != null)
            SoundManager.Instance.PlaySFX(criticalAttackSound);
    }

    private void SpawnEffect(GameObject prefab, Vector3 position, HitEffectConfig config)
    {
        Vector3 offset = config != null ? config.positionOffset : Vector3.zero;
        Quaternion rotation =
            config != null ? Quaternion.Euler(config.rotation) : Quaternion.identity;
        Vector3 scale = config != null ? config.scale : Vector3.one;

        var instance = Instantiate(prefab, position + offset, rotation);
        instance.transform.localScale = scale;
    }

    public void PlayHealEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, position, Quaternion.identity);

        SoundManager.Instance.PlaySFX(HealSound);
    }

    public void PlayAnimation(Animator animator, string trigger)
    {
        Debug.Assert(animator != null, "Animator is null in VisualEffectManager.PlayAnimation");
        if (animator != null)
            animator.SetTrigger(trigger);
    }

    public void ShowFloatingText(string text, Vector3 position, Color color)
    {
        if (canvasTransform == null)
            return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            screenPos,
            null,
            out anchoredPos
        );

        GameObject floatingText;
        if (floatingTextPrefab != null)
        {
            floatingText = Instantiate(floatingTextPrefab, canvasTransform);
            Debug.Log("teste");
            var rt = floatingText.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            floatingText.GetComponent<FloatingText>().ShowNumber(int.Parse(text), color);
        }
    }
}
