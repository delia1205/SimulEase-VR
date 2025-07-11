using UnityEngine;

public class CafeAmbienceManager : MonoBehaviour
{
    public AudioSource easyAmbienceSource;
    public AudioSource hardAmbienceSource;

    void Start()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Beginner");

        switch (difficulty)
        {
            case "Beginner":
            case "Mid":
                if (easyAmbienceSource != null)
                {
                    easyAmbienceSource.Play();
                }
                if (hardAmbienceSource != null)
                {
                    hardAmbienceSource.Stop();
                }
                break;

            case "Advanced":
                if (hardAmbienceSource != null)
                {
                    hardAmbienceSource.Play();
                }
                if (easyAmbienceSource != null)
                {
                    easyAmbienceSource.Stop();
                }
                break;

            default:
                Debug.LogWarning("Unknown difficulty level: " + difficulty);
                break;
        }
    }
}
