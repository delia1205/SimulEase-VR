using UnityEngine;

public class EyeContactMonitor : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;
    public Transform alinaHead;
    public GazeFollowingHUD gazeHUD;

    [Header("Settings")]
    [Range(0.8f, 1f)]
    public float eyeContactThreshold = 0.95f;

    public float distractionDuration = 2f;

    private float distractionTimer = 0f;
    private bool inEyeContact = false;

    void Update()
    {
        if (playerHead == null || alinaHead == null) return;

        Vector3 toAlina = (alinaHead.position - playerHead.position);
        Vector3 playerForward = playerHead.forward;

        float dot = Vector3.Dot(playerForward, toAlina);

        if (dot >= eyeContactThreshold)
        {
            inEyeContact = true;
            distractionTimer = 0f;
        }
        else
        {
            distractionTimer += Time.deltaTime;
            if (inEyeContact && distractionTimer >= distractionDuration)
            {
                inEyeContact = false;
            }
            if(distractionTimer >= distractionDuration)
            {
                gazeHUD.ShowMessage("Try to maintain eye contact with Alina when talking to her.");
            }
            
        }
    }
}
