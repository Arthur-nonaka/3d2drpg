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

    public void ShowNumber(int number)
    {
        string numStr = number.ToString();
        float startX = -(numStr.Length - 1) * digitSpacing / 2f;

        for (int i = 0; i < numStr.Length; i++)
        {
            int digit = numStr[i] - '0';
            GameObject digitGO = Instantiate(digitPrefab, digitContainer);
            digitGO.transform.localPosition = new Vector3(startX + i * digitSpacing, 0, 0);

            var sr = digitGO.GetComponent<SpriteRenderer>();
            sr.sprite = digits[digit];
            sr.color = new Color(1, 1, 1, 0);

            float delay = initialDelay * i;
            sr.DOFade(1, digitAppearTime).SetDelay(delay);
            digitGO
                .transform.DOScale(Vector3.one * 1.2f, digitAppearTime)
                .SetDelay(delay)
                .OnComplete(() => digitGO.transform.DOScale(Vector3.one, 0.1f));
        }

        transform.DOMoveY(transform.position.y + 1.5f, 2f);
        Destroy(gameObject, 1.5f);
    }
}
