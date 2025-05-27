using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialAnimationController : MonoBehaviour
{
    [Tooltip("Assign all the animation GameObjects in the correct sequence")]
    public List<GameObject> animationSteps;

    [Tooltip("Minimum random idle time before stand up starts (seconds)")]
    public float minIdleTime = 2f;

    [Tooltip("Maximum random idle time before stand up starts (seconds)")]
    public float maxIdleTime = 6f;


    void Start()
    {
        StartCoroutine(PlayAnimationSequence());
    }

    IEnumerator PlayAnimationSequence()
    {
        float idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);

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
    }

    AnimationClip GetCurrentAnimationClip(Animator animator)
    {
        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        if (rac == null || rac.animationClips.Length == 0) return null;

        return rac.animationClips[0];
    }
}
