using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeSequenceManager : MonoBehaviour
{
    public List<CafeAnimationSequence> sequences;
    [Tooltip("Min idle time before triggering a random sequence")]
    public float minIdleTime = 5f;
    [Tooltip("Max idle time before triggering a random sequence")]
    public float maxIdleTime = 10f;

    private bool isPlaying = false;

    void Start()
    {
        StartCoroutine(SequenceLoop());
    }

    IEnumerator SequenceLoop()
    {
        while (true)
        {
            if (!isPlaying)
            {
                float wait = Random.Range(minIdleTime, maxIdleTime);
                yield return new WaitForSeconds(wait);

                CafeAnimationSequence selected = GetRandomSequence();
                if (selected != null)
                {
                    isPlaying = true;
                    yield return StartCoroutine(selected.PlaySequence());
                    isPlaying = false;
                }
            }

            yield return null;
        }
    }

    // TODO: add weights to some sequences that are more probable to happen

    CafeAnimationSequence GetRandomSequence()
    {
        if (sequences.Count == 0) return null;
        return sequences[Random.Range(0, sequences.Count)];
    }
}
