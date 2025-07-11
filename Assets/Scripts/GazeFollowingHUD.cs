using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GazeFollowingHUD : MonoBehaviour
{
    public Transform playerHead;                 
    public float followDistance = 2f;            
    public float lerpSpeed = 5f;                 
    public CanvasGroup canvasGroup;          
    public TextMeshProUGUI messageText;         
    public float fadeDuration = 0.5f;         
    public float displayDuration = 5f;        

    private float timer = 0f;
    private bool isShowing = false;

    void Update()
    {
        if (playerHead == null) return;

        Vector3 targetPos = playerHead.position + playerHead.forward * followDistance;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);

        transform.LookAt(playerHead);
        transform.Rotate(0, 180, 0);

        if (isShowing)
        {
            timer += Time.deltaTime;
            if (timer >= displayDuration)
            {
                StartCoroutine(FadeOut());
            }
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        timer = 0f;

        if (!isShowing)
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        isShowing = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        isShowing = false;
        float t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        messageText.text = "";
    }
}
