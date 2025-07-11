using UnityEngine;

public class UIButton : MonoBehaviour
{
    public string scenarioName;

    public void OnScenarioSelected()
    {
        ScenarioSelection.Instance.SelectScenario(scenarioName);
        Debug.Log("Scenario selected: " + scenarioName);
        ScenarioSelection.Instance.StartScene();
    }

}
