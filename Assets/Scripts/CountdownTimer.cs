using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float remainingTime;
    public Image fadeImage;
    public float fadeDuration = 1.5f;
    public MonoBehaviour emotionListenerComponent;
    private IEmotionProvider emotionListener;

    void Start()
    {
        emotionListener = emotionListenerComponent as IEmotionProvider;

        string durationStr = PlayerPrefs.GetString("SelectedDuration", "5");
        // string durationStr = "1";
        if (float.TryParse(durationStr, out float durationMinutes))
        {
            remainingTime = durationMinutes * 60f;
        }
        else
        {
            remainingTime = 300f;
        }

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            remainingTime = 0f;
            UpdateTimerUI();
            StartCoroutine(FadeAndLoadScene());
            enabled = false;
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    IEnumerator FadeAndLoadScene()
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("Fade image not set. Loading statistics scene directly.");
            SaveEmotionData();
            SceneManager.LoadScene("StatisticsScene");
            yield break;
        }

        Color color = fadeImage.color;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SaveEmotionData();
        SceneManager.LoadScene("StatisticsScene");
    }

    void SaveEmotionData()
    {
        if (emotionListener != null)
        {
            EmotionStats.emotionFrequency = new Dictionary<string, int>(emotionListener.GetEmotionFrequency());
        }
    }
}
