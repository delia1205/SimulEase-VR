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
        ApplyDifficultySettings();
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

    CafeAnimationSequence GetRandomSequence()
    {
        if (sequences.Count == 0) return null;
        return sequences[Random.Range(0, sequences.Count)];
    }

    void ApplyDifficultySettings()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Beginner");

        switch (difficulty)
        {
            case "Beginner":
                minIdleTime = 240f;
                maxIdleTime = 300f;
                break;
            case "Mid":
                minIdleTime = 180f;
                maxIdleTime = 240f;
                break;
            case "Advanced":
                minIdleTime = 120f;
                maxIdleTime = 180f;
                break;
            default:
                Debug.LogWarning("Unknown difficulty level, using default idle times.");
                break;
        }
    }
}
