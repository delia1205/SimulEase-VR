using UnityEngine;
using System.Collections;

public class TalkTestStarter : MonoBehaviour
{
    public TalkingAlina talkAnimator;

    void Start()
    {
        if (talkAnimator != null)
        {
            StartCoroutine(TalkTest());
        }
        else
        {
            Debug.LogError("TalkTestStarter: No Talk Animator assigned.");
        }
    }

    IEnumerator TalkTest()
    {
        talkAnimator.StartTalking();
        Debug.Log("Talking started.");

        yield return new WaitForSeconds(20f);

        talkAnimator.StopTalking();
        Debug.Log("Talking stopped.");
    }
}
