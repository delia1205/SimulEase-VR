using UnityEngine;
using UnityEngine.UI;

public class DurationSelection : MonoBehaviour
{
    public string duration;
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
            scenarioSelection.SelectDuration(duration);
            PlayerPrefs.SetString("SelectedDuration", duration);
            PlayerPrefs.Save();
            Debug.Log("Duration set to: " + duration);
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
