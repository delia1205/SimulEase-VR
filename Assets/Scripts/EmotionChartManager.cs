using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EmotionChartManager : MonoBehaviour
{
    public GameObject emotionBarPrefab;   
    public Transform chartContainer;
    public TextMeshProUGUI durationText;

    void Start()
    {
        string durationStr = PlayerPrefs.GetString("SelectedDuration", "5");
        durationText.text = durationStr + " minutes";
        Dictionary<string, int> rawFrequencies = EmotionStats.emotionFrequency;

        if (rawFrequencies == null || rawFrequencies.Count == 0)
        {
            Debug.LogWarning("No emotion data available.");
            return;
        }

        int total = rawFrequencies.Values.Sum();
        if (total == 0) return;

        var sorted = rawFrequencies
            .OrderByDescending(kvp => kvp.Value)
            .ToList();

        Dictionary<string, float> chartData = new Dictionary<string, float>();

        for (int i = 0; i < sorted.Count; i++)
        {
            string emotion = sorted[i].Key;
            float rawPercent = (sorted[i].Value / (float)total) * 100f;
            float percent = Mathf.Floor(rawPercent * 100f) / 100f;

            if (i < 5)
                chartData[ToTitleCase(emotion)] = percent;
            else
                chartData["Others"] = chartData.ContainsKey("Others") ? chartData["Others"] + percent : percent;
        }

        foreach (var entry in chartData)
        {
            GameObject bar = Instantiate(emotionBarPrefab, chartContainer);
            EmotionBar barScript = bar.GetComponent<EmotionBar>();
            barScript.SetEmotionData(entry.Key, entry.Value);
        }
    }

    string ToTitleCase(string input)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }

}
