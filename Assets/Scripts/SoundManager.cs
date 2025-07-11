using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] audienceSounds;

    [Header("Audio Settings")]
    public float minDelay = 3f;
    public float maxDelay = 10f;
    public Vector3 audienceAreaCenter;
    public GameObject planeObject;
    Vector3 audienceAreaSize;
    public bool use3DSound = true;

    private void Start()
    {
        SetDelaysBasedOnDifficulty();
        StartCoroutine(PlayRandomSoundsLoop());
        Vector3 planeScale = planeObject.transform.localScale;
        audienceAreaSize = new Vector3(10f * planeScale.x, 1f, 10f * planeScale.z);
}

    IEnumerator PlayRandomSoundsLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (audienceSounds.Length > 0)
            {
                AudioClip clip = audienceSounds[Random.Range(0, audienceSounds.Length)];
                PlaySound(clip);
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (use3DSound)
        {
            Vector3 randomPos = audienceAreaCenter + new Vector3(
                Random.Range(-audienceAreaSize.x / 2, audienceAreaSize.x / 2),
                Random.Range(-audienceAreaSize.y / 2, audienceAreaSize.y / 2),
                Random.Range(-audienceAreaSize.z / 2, audienceAreaSize.z / 2)
            );

            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = randomPos;
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f; 
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 15f;
            audioSource.Play();

            Destroy(tempAudio, clip.length + 0.5f);
        }
        else
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(audienceAreaCenter, audienceAreaSize);
    }

    void SetDelaysBasedOnDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Beginner");

        switch (difficulty)
        {
            case "Beginner":
                minDelay = 30f;
                maxDelay = 60f;
                break;
            case "Mid":
                minDelay = 10f;
                maxDelay = 15f;
                break;
            case "Advanced":
                minDelay = 3f;
                maxDelay = 10f;
                break;
            default:
                Debug.LogWarning("Unknown difficulty: " + difficulty);
                break;
        }
    }
}
