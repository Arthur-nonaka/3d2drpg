using DG.Tweening;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Sprite[] digits;

    [SerializeField]
    private Transform digitContainer;
    public GameObject digitPrefab;
    private Color textColor;
    public float digitSpacing;
    public float initialDelay;
    public float digitAppearTime;

    public void ShowNumber(int number, Color color)
    {
        string numStr = number.ToString();
        float startX = -(numStr.Length - 1) * digitSpacing / 2f;

        for (int i = 0; i < numStr.Length; i++)
        {
            int digit = numStr[i] - '0';
            GameObject digitGO = Instantiate(digitPrefab, digitContainer);
            digitGO.transform.localPosition = new Vector3(startX + i * digitSpacing, 0, 0);

            var sr = digitGO.GetComponent<SpriteRenderer>();

            sr.material = new Material(sr.material);
            sr.sprite = digits[digit];
            sr.material.SetTexture("_EmissionMap", digits[digit].texture);
            sr.material.SetColor("_EmissionColor", new Color(color.r, color.g, color.b, 0));
            sr.material.SetColor("_EmissionColor", new Color(color.r, color.g, color.b, 0));

            float delay = initialDelay * i;
            DOTween
                .To(
                    () => sr.material.GetColor("_EmissionColor"),
                    x => sr.material.SetColor("_EmissionColor", x),
                    color,
                    digitAppearTime
                )
                .SetDelay(delay);
            digitGO
                .transform.DOScale(Vector3.one * 1.2f, digitAppearTime)
                .SetDelay(delay)
                .OnComplete(() => digitGO.transform.DOScale(Vector3.one, 0.1f));
        }

        transform.DOMoveY(transform.position.y + 1.5f, 1.3f).SetEase(Ease.InOutBack);
        Destroy(gameObject, 1.3f);
    }

    void OnDestroy()
    {
        transform.DOKill();
        foreach (Transform child in digitContainer)
        {
            child.DOKill();
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && sr.material != null)
                Destroy(sr.material);
        }
    }
}
