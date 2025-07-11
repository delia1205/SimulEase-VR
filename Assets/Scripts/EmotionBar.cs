using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmotionBar : MonoBehaviour
{
    public TextMeshProUGUI emotionNameText;
    public Image barImage;
    public TextMeshProUGUI percentageText;

    public float maxBarWidth = 500f;

    public void SetEmotionData(string emotion, float percentage)
    {
        emotionNameText.text = emotion;
        percentageText.text = $"{percentage}%";

        float width = (percentage / 100f) * maxBarWidth;
        barImage.rectTransform.sizeDelta = new Vector2(width, barImage.rectTransform.sizeDelta.y);
    }
}
