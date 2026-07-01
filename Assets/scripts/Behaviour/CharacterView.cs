using DG.Tweening;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public string Name { get; private set; }
    public AttackConfig AttackConfig => attackConfig;

    [SerializeField]
    private AttackConfig attackConfig;
    private int pendingDamage;
    private Vector3 originalPosition;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private System.Action<int, HitEffectConfig> onHitCallback;
    private System.Action onReturnCompleteCallback;
    private bool attackEnded;

    private SpriteRenderer spriteRenderer;
    public Sprite Sprite => spriteRenderer?.sprite;
    public Sprite DefaultSprite { get; private set; }
    private Material originalMaterial;

    void Awake()
    {
        Animator = GetComponent<Animator>();
        Name = gameObject.name;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.sharedMaterial;
            DefaultSprite = spriteRenderer.sprite;
        }
    }

    public void PerformAttackMove(
        Vector3 originalPosition,
        Vector3 targetPosition,
        int damage,
        System.Action<int, HitEffectConfig> onAttack,
        System.Action onReturnComplete
    )
    {
        onHitCallback = onAttack;
        onReturnCompleteCallback = onReturnComplete;
        pendingDamage = damage;
        this.originalPosition = originalPosition;
        attackEnded = false;

        var config = attackConfig;

        if (config.useWalk)
        {
            Vector3 moveDirection = (targetPosition - originalPosition).normalized;

            cameraForward = Camera.main.transform.forward;
            cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            float moveX = Vector3.Dot(moveDirection, cameraRight);
            float moveY = Vector3.Dot(moveDirection, cameraForward);

            Animator.SetFloat("MoveX", moveX);
            Animator.SetFloat("MoveY", moveY);
            Animator.SetBool("IsWalking", true);
            Sequence attackSequence = DOTween.Sequence();
            attackSequence
                .Append(
                    transform
                        .DOMove(
                            targetPosition + (Vector3.up * transform.position.y),
                            config.approachDuration
                        )
                        .SetEase(config.approachEase)
                )
                .AppendCallback(() =>
                {
                    Animator.SetBool("IsWalking", false);
                    Animator.SetTrigger("Attack");
                    Invoke(nameof(SafeEndAttack), 2f);
                });
            return;
        }
        else
        {
            Animator.SetTrigger("Attack");
        }
    }

    public void OnAttackHit(HitEffectConfig config)
    {
        onHitCallback?.Invoke(pendingDamage, config);
    }

    public void OnAttackEnd()
    {
        if (attackEnded)
            return;
        attackEnded = true;
        CancelInvoke(nameof(SafeEndAttack));

        if (attackConfig == null)
        {
            onReturnCompleteCallback?.Invoke();
            return;
        }

        Vector3 returnDirection = (originalPosition - transform.position).normalized;
        float moveX = Vector3.Dot(returnDirection, cameraRight);
        float moveY = Vector3.Dot(returnDirection, cameraForward);

        Animator.SetFloat("MoveX", moveX);
        Animator.SetFloat("MoveY", moveY);
        Animator.SetBool("IsWalking", true);

        Sequence returnSeq = DOTween.Sequence();
        returnSeq
            .Append(
                transform
                    .DOMove(originalPosition, attackConfig.returnDuration)
                    .SetEase(attackConfig.returnEase)
            )
            .OnComplete(() =>
            {
                Animator.SetFloat("MoveX", 0);
                Animator.SetFloat("MoveY", 0);
                Animator.SetBool("IsWalking", false);
                onReturnCompleteCallback?.Invoke();
            });
    }

    public void PerformHitReaction(Vector3 originalPosition, int direction = 1)
    {
        Animator.SetTrigger("Hit");

        if (spriteRenderer != null && originalMaterial != null)
        {
            var flashMat = new Material(originalMaterial);
            flashMat.color = Color.red;
            spriteRenderer.material = flashMat;

            DOTween
                .To(() => flashMat.color, x => flashMat.color = x, Color.white, 0.4f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    if (spriteRenderer != null)
                        spriteRenderer.material = originalMaterial;
                    Destroy(flashMat);
                });
        }

        Sequence hitSequence = DOTween.Sequence();
        hitSequence
            .Append(
                transform
                    .DOMove(originalPosition + (Vector3.right * direction * 1f), 0.2f)
                    .SetEase(Ease.OutCirc)
            )
            .AppendInterval(0.3f)
            .Append(transform.DOMove(originalPosition, 0.2f));
    }

    public void PlayDeathAnimation()
    {
        Animator.SetBool("Dead", true);
    }

    private void SafeEndAttack()
    {
        if (!attackEnded)
            OnAttackEnd();
    }

    void OnDestroy()
    {
        CancelInvoke();
        DOTween.Kill(transform);
    }
}
