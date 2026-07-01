using UnityEngine;

public class IdleAnimationRandomizer : MonoBehaviour
{
    [SerializeField]
    private float minPause = 1f;

    [SerializeField]
    private float maxPause = 5f;

    [SerializeField]
    private string idleStateName = "Idle";

    private Animator animator;
    private float pauseTimer = -1f;
    private float currentPause;
    private int currentLoopCount;
    private int lastLoopCount;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        pauseTimer = -1f;
        lastLoopCount = -1;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName(idleStateName))
        {
            pauseTimer = -1f;
            lastLoopCount = -1;
            return;
        }

        if (stateInfo.loop)
        {
            currentLoopCount = Mathf.FloorToInt(stateInfo.normalizedTime);
            if (currentLoopCount > lastLoopCount)
            {
                lastLoopCount = currentLoopCount;
                StartPause();
                animator.speed = 0f;
            }

            if (pauseTimer >= 0f)
            {
                pauseTimer += Time.deltaTime;
                if (pauseTimer >= currentPause)
                {
                    animator.speed = 1f;
                    pauseTimer = -1f;
                }
            }
        }
        else
        {
            if (stateInfo.normalizedTime >= 1f && pauseTimer < 0f)
            {
                StartPause();
            }

            if (pauseTimer >= 0f)
            {
                pauseTimer += Time.deltaTime;
                if (pauseTimer >= currentPause)
                {
                    animator.Play(stateInfo.fullPathHash, 0, 0f);
                    pauseTimer = -1f;
                }
            }
        }
    }

    private void StartPause()
    {
        currentPause = Random.Range(minPause, maxPause);
        pauseTimer = 0f;
    }
}
