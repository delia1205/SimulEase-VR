using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialAnimationController : MonoBehaviour
{
    [Tooltip("Assign all the animation GameObjects in the correct sequence")]
    public List<GameObject> animationSteps;

    [Tooltip("Minimum random idle time before sequence starts (seconds)")]
    public float minIdleTime = 2f;

    [Tooltip("Maximum random idle time before sequence starts (seconds)")]
    public float maxIdleTime = 6f;

    public AudioSource audioSource;
    public AudioClip audioLoopClip;

    void Start()
    {
        SetIdleTimesBasedOnDifficulty();
        StartCoroutine(PlayAnimationSequence());
    }

    void StartAudio()
    {
        if (audioSource != null && audioLoopClip != null)
        {
            audioSource.clip = audioLoopClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }


    IEnumerator PlayAnimationSequence()
    {
        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);

        StartAudio();

        foreach (GameObject obj in animationSteps)
        {
            if (obj == null) continue;

            obj.SetActive(true);

            Animator animator = obj.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("No Animator found on " + obj.name);
                continue;
            }

            AnimationClip clip = GetCurrentAnimationClip(animator);
            float duration = clip != null ? clip.length : 1f;
            yield return new WaitForSeconds(duration);

            obj.SetActive(false);
        }

        StopAudio();
    }

    AnimationClip GetCurrentAnimationClip(Animator animator)
    {
        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        if (rac == null || rac.animationClips.Length == 0) return null;

        return rac.animationClips[0];
    }

    void SetIdleTimesBasedOnDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Beginner");

        switch (difficulty)
        {
            case "Beginner":
                minIdleTime = 360f;
                maxIdleTime = 480f;
                break;
            case "Mid":
                minIdleTime = 240f;
                maxIdleTime = 300f;
                break;
            case "Advanced":
                minIdleTime = 120f;
                maxIdleTime = 240f;
                break;
            default:
                Debug.LogWarning("Unknown difficulty: " + difficulty);
                break;
        }
    }
}
