using UnityEngine;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    public string difficultyLevel;
    public ScenarioSelection scenarioSelection;
    private Toggle myToggle;

    void Start()
    {
        myToggle = GetComponent<Toggle>();

        if (myToggle != null)
        {
            myToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        else
        {
            Debug.LogWarning("Toggle component not found on this GameObject.");
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            scenarioSelection.SelectDifficulty(difficultyLevel);
            PlayerPrefs.SetString("SelectedDifficulty", difficultyLevel);
            PlayerPrefs.Save();
            Debug.Log("Difficulty set to: " + difficultyLevel);
        }
    }

    void OnDestroy()
    {
        if (myToggle != null)
        {
            myToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}
