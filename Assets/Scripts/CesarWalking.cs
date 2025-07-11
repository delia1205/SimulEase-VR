using UnityEngine;

public class CesarWalking : MonoBehaviour
{
    public Animator animator;
    public AudioSource footstepAudio;
    public AudioClip footstepClip;
    public float minTime = 10f;
    public float maxTime = 30f;

    private void Start()
    {
        SetDelayTimesBasedOnDifficulty();
        gameObject.SetActive(false);
        float randomDelay = Random.Range(minTime, maxTime);
        Invoke("AppearAndAnimate", randomDelay);
    }

    void AppearAndAnimate()
    {
        gameObject.SetActive(true);
        animator.Play("Walk_Inspection_M");
        footstepAudio.clip = footstepClip;
        footstepAudio.loop = true;
        footstepAudio.Play();
        Invoke("Disappear", animator.GetCurrentAnimatorStateInfo(0).length);
    }

    void Disappear()
    {
        CancelInvoke("PlayFootstepSound");
        gameObject.SetActive(false);
    }

    void SetDelayTimesBasedOnDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Beginner");

        switch (difficulty)
        {
            case "Beginner":
                minTime = 180f;
                maxTime = 360f;
                break;
            case "Mid":
                minTime = 120f;
                maxTime = 180f;
                break;
            case "Advanced":
                minTime = 60f;
                maxTime = 120f;
                break;
            default:
                Debug.LogWarning("Unknown difficulty: " + difficulty);
                break;
        }
    }
}

