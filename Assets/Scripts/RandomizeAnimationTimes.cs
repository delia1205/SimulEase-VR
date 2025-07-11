using UnityEngine;

public class RandomizeAnimationTimes : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.speed = Random.Range(0.8f, 1.2f);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.Range(0f, 1f));
        }
    }
}
