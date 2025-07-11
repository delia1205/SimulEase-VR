using System.Collections.Generic;
using UnityEngine;

public class EmotionBarchartPlaceholder : MonoBehaviour
{
    public GameObject emotionBarPrefab;   
    public Transform chartContainer;     

    void Start()
    {
        // Placeholder emotion percentages
        Dictionary<string, float> chartData = new Dictionary<string, float>
        {
            { "Joy", 35f },
            { "Anxiety", 25f },
            { "Confusion", 15f },
            { "Anger", 10f },
            { "Surprise", 8f },
            { "Others", 7f }
        };

        foreach (var entry in chartData)
        {
            GameObject bar = Instantiate(emotionBarPrefab, chartContainer);
            EmotionBar barScript = bar.GetComponent<EmotionBar>();
            barScript.SetEmotionData(entry.Key, entry.Value);
        }
    }
}


