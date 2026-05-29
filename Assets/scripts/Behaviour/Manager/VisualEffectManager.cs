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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayAttackEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, position, Quaternion.identity);

        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, position);
    }

    public void PlayAnimation(Animator animator, string trigger)
    {
        Debug.Assert(animator != null, "Animator is null in VisualEffectManager.PlayAnimation");
        if (animator != null)
            animator.SetTrigger(trigger);
    }

    public void ShowFloatingText(string text, Vector3 position, Color color)
    {
        if (floatingTextPrefab != null)
        {
            GameObject floatingText = Instantiate(
                floatingTextPrefab,
                position,
                Quaternion.identity
            );
            floatingText.GetComponent<FloatingText>().ShowNumber(int.Parse(text));
        }
    }
}
