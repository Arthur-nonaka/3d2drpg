using DG.Tweening;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public string Name { get; private set; }

    void Awake()
    {
        Animator = GetComponent<Animator>();
        Name = gameObject.name;
    }

    public void PerformAttackMove(
        Vector3 originalPosition,
        Vector3 targetPosition,
        System.Action onAttack,
        System.Action onReturnComplete
    )
    {
        Sequence attackSequence = DOTween.Sequence();
        attackSequence
            .Append(
                transform
                    .DOMove(targetPosition + (Vector3.up * transform.position.y), 0.4f)
                    .SetEase(Ease.Linear)
            )
            .AppendCallback(() => onAttack?.Invoke())
            .AppendInterval(1f)
            .Append(transform.DOMove(originalPosition, 0.4f))
            .OnComplete(() => onReturnComplete?.Invoke());
    }

    // public void PlayAttackAnimation()
    // {
    //     animator.SetTrigger("Attack");
    // }
}
