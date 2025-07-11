using System.Collections;
using UnityEngine;

public class AlinaRandomGestures : MonoBehaviour
{
    public Animator animator;
    public float minTime = 5f;
    public float maxTime = 15f;

    private string[] gestures = { "HeadNod", "HeadTilt", "HeadRotate" };

    void Start()
    {
        StartCoroutine(PlayRandomGesture());
    }

    IEnumerator PlayRandomGesture()
    {
        while (true)
        {
            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);

            string gesture = gestures[Random.Range(0, gestures.Length)];
            animator.SetTrigger(gesture);
        }
    }
}
