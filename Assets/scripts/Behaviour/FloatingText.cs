using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    [SerializeField]
    private float floatDistance = 80f;

    [SerializeField]
    private float duration = 1.3f;

    public void ShowNumber(int number, Color color)
    {
        textMesh.text = number.ToString();
        textMesh.color = color;

        transform.DOBlendableLocalMoveBy(new Vector3(0, floatDistance, 0), duration)
            .SetEase(Ease.OutCubic);
        textMesh.DOFade(0, duration * 0.7f).SetDelay(duration * 0.3f);

        Destroy(gameObject, duration);
    }
}
