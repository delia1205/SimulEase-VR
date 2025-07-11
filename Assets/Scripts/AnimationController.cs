using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    private float idleTimer;

    void Start()
    {
        SetIdleTimesBasedOnDifficulty();
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

    void SetIdleTimesBasedOnDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Beginner");

        switch (difficulty)
        {
            case "Beginner":
                minIdleTime = 240f;
                maxIdleTime = 360f;
                break;
            case "Mid":
                minIdleTime = 120f;
                maxIdleTime = 240f;
                break;
            case "Advanced":
                minIdleTime = 60f;
                maxIdleTime = 120f;
                break;
            default:
                Debug.LogWarning("Unknown difficulty setting: " + difficulty);
                break;
        }
    }
}
