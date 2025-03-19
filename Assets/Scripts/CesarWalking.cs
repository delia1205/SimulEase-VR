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
}

