using UnityEngine;
using UnityEngine.UI;

public class UIToggleTest : MonoBehaviour
{
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
        Debug.Log("Toggle changed! New value: " + isOn);
    }

    void OnDestroy()
    {
        if (myToggle != null)
        {
            myToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}
