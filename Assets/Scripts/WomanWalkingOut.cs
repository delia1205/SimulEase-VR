using UnityEngine;

public class WomanWalkingOut : MonoBehaviour
{
    public Transform pointAfterWalkRight;
    public Transform pointAfterWalkStraight;
    public float walkSpeed = 1.5f;

    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    private Animator animator;
    private int phase = 0;
    private bool sequenceStarted = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        float randomDelay = Random.Range(minIdleTime, maxIdleTime);
        Invoke(nameof(StartSequence), randomDelay);
    }

    void Update()
    {
        if (!sequenceStarted) return;

        if (phase == 1)
        {
            MoveTowards(pointAfterWalkRight.position);

            if (Vector3.Distance(transform.position, pointAfterWalkRight.position) < 0.05f)
            {
                phase = 2;
                animator.SetTrigger("turn");
            }
        }

        if (phase == 3)
        {
            MoveTowards(pointAfterWalkStraight.position);

            if (Vector3.Distance(transform.position, pointAfterWalkStraight.position) < 0.05f)
            {
                phase = 4;
            }
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * walkSpeed * Time.deltaTime;
    }

    void StartSequence()
    {
        if (sequenceStarted) return;
        sequenceStarted = true;

        animator.SetTrigger("startSequence");
        phase = 1;
    }

    public void OnTurnFinished()
    {
        animator.SetTrigger("walkStraight");
        phase = 3;
    }
}
