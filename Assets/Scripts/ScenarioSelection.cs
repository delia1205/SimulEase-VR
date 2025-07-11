using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioSelection : MonoBehaviour
{
    public static ScenarioSelection Instance;

    public string selectedScenario;
    public string selectedDifficulty;
    public string selectedDuration;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectScenario(string scenario)
    {
        selectedScenario = scenario;
    }

    public void SelectDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
    }

    public void SelectDuration(string duration)
    {
        selectedDuration = duration;
    }

    public void StartScene()
    {
        if (string.IsNullOrEmpty(selectedScenario))
        {
            Debug.LogError("Scenario not selected.");
            return;
        }

        if (string.IsNullOrEmpty(selectedDifficulty))
        {
            selectedDifficulty = "Beginner";
            PlayerPrefs.SetString("SelectedDifficulty", selectedDifficulty);
        }

        if (string.IsNullOrEmpty(selectedDuration))
        {
            selectedDuration = "5";
            PlayerPrefs.SetString("SelectedDuration", selectedDuration);
        }

        PlayerPrefs.Save();
        Debug.Log($"Loading scene {selectedScenario} with difficulty {selectedDifficulty} and duration {selectedDuration} mins");
        LoadingManager.Instance.LoadScene(selectedScenario + "Scene");
    }

}
