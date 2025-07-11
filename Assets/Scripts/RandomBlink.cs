using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BlendshapeTarget
{
    public SkinnedMeshRenderer renderer;
    [HideInInspector] public int tiltIndex = -1;
}

public class RandomBlink : MonoBehaviour
{
    public SkinnedMeshRenderer faceRenderer;
    public string leftEyeBlinkName = "Eye_Blink_L";
    public string rightEyeBlinkName = "Eye_Blink_R";
    public float minBlinkInterval = 3f;
    public float maxBlinkInterval = 8f;
    public float blinkDuration = 0.15f;

    public List<BlendshapeTarget> tiltTargets;
    public string headTiltBlendshapeName = "Head-Tilt_L";
    public float minTiltInterval = 6f;
    public float maxTiltInterval = 15f;
    public float tiltDuration = 0.5f;
    public float tiltHoldTime = 0.1f;

    private int leftEyeIndex;
    private int rightEyeIndex;

    void Start()
    {
        leftEyeIndex = faceRenderer.sharedMesh.GetBlendShapeIndex(leftEyeBlinkName);
        rightEyeIndex = faceRenderer.sharedMesh.GetBlendShapeIndex(rightEyeBlinkName);

        if (leftEyeIndex == -1 || rightEyeIndex == -1)
        {
            Debug.LogError("Blendshape name not found on mesh!");
            return;
        }

        foreach (var target in tiltTargets)
        {
            if (target.renderer == null) continue;

            int index = target.renderer.sharedMesh.GetBlendShapeIndex(headTiltBlendshapeName);
            if (index == -1)
            {
                Debug.LogWarning($"'{headTiltBlendshapeName}' not found on {target.renderer.name}");
            }

            target.tiltIndex = index;
        }

        StartCoroutine(TiltRoutine());

        StartCoroutine(BlinkRoutine());
    }

    IEnumerator TiltRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTiltInterval, maxTiltInterval);
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(DoHeadTilt());
        }
    }

    IEnumerator DoHeadTilt()
    {
        float halfDuration = tiltDuration / 2f;

        float timer = 0f;
        while (timer < halfDuration)
        {
            float weight = Mathf.Lerp(0f, 100f, timer / halfDuration);
            foreach (var target in tiltTargets)
            {
                if (target.tiltIndex != -1)
                    target.renderer.SetBlendShapeWeight(target.tiltIndex, weight);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var target in tiltTargets)
        {
            if (target.tiltIndex != -1)
                target.renderer.SetBlendShapeWeight(target.tiltIndex, 100f);
        }

        yield return new WaitForSeconds(tiltHoldTime);

        timer = 0f;
        while (timer < halfDuration)
        {
            float weight = Mathf.Lerp(100f, 0f, timer / halfDuration);
            foreach (var target in tiltTargets)
            {
                if (target.tiltIndex != -1)
                    target.renderer.SetBlendShapeWeight(target.tiltIndex, weight);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var target in tiltTargets)
        {
            if (target.tiltIndex != -1)
                target.renderer.SetBlendShapeWeight(target.tiltIndex, 0f);
        }
    }


    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(BlinkOnce());
        }
    }

    IEnumerator BlinkOnce()
    {
        float halfDuration = blinkDuration / 2f;
        float holdTime = 0.03f; 

        float timer = 0f;
        while (timer < halfDuration)
        {
            float weight = Mathf.Lerp(0f, 100f, timer / halfDuration);
            faceRenderer.SetBlendShapeWeight(leftEyeIndex, weight);
            faceRenderer.SetBlendShapeWeight(rightEyeIndex, weight);
            timer += Time.deltaTime;
            yield return null;
        }

        faceRenderer.SetBlendShapeWeight(leftEyeIndex, 100f);
        faceRenderer.SetBlendShapeWeight(rightEyeIndex, 100f);
        yield return new WaitForSeconds(holdTime);

        timer = 0f;
        while (timer < halfDuration)
        {
            float weight = Mathf.Lerp(100f, 0f, timer / halfDuration);
            faceRenderer.SetBlendShapeWeight(leftEyeIndex, weight);
            faceRenderer.SetBlendShapeWeight(rightEyeIndex, weight);
            timer += Time.deltaTime;
            yield return null;
        }

        faceRenderer.SetBlendShapeWeight(leftEyeIndex, 0f);
        faceRenderer.SetBlendShapeWeight(rightEyeIndex, 0f);
    }

}
