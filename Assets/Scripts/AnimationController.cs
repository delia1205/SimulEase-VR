using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    private float idleTimer;

    void Start()
    {
        ResetIdleTimer();
    }

    void Update()
    {
        idleTimer -= Time.deltaTime;

        // If the idle timer reaches zero, start the sequence
        if (idleTimer <= 0)
        {
            PlaySequence();
            ResetIdleTimer();
        }
    }

    void PlaySequence()
    {
        animator.SetTrigger("startSequence");
    }

    void ResetIdleTimer()
    {
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }
}
