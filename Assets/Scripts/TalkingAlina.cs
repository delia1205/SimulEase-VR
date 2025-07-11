using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkingAlina : MonoBehaviour
{
    [System.Serializable]
    public class VisemeBlendshape
    {
        public string name;
        public float maxWeight = 100f;
    }

    public SkinnedMeshRenderer faceRenderer;

    public List<VisemeBlendshape> visemeShapes = new List<VisemeBlendshape>()
    {
        new VisemeBlendshape { name = "V_Open", maxWeight = 100f },
        new VisemeBlendshape { name = "V_Explosive", maxWeight = 100f },
        new VisemeBlendshape { name = "V_Tight_O", maxWeight = 100f },
        new VisemeBlendshape { name = "V_Wide", maxWeight = 100f },
        new VisemeBlendshape { name = "V_Affricate", maxWeight = 100f },
        new VisemeBlendshape { name = "V_Lip_Open", maxWeight = 100f },
        new VisemeBlendshape { name = "Mouth_Funnel", maxWeight = 40f }
    };

    public float minInterval = 0.05f;
    public float maxInterval = 0.15f;
    public float blendSpeed = 8f;
    public bool talking = false;

    private Dictionary<string, int> shapeIndices = new Dictionary<string, int>();
    private string currentShape = null;
    private float currentWeight = 0f;
    private Coroutine talkRoutine;

    void Start()
    {
        foreach (var shape in visemeShapes)
        {
            int index = faceRenderer.sharedMesh.GetBlendShapeIndex(shape.name);
            if (index != -1)
                shapeIndices[shape.name] = index;
            else
                Debug.LogWarning($"Blendshape '{shape.name}' not found on {faceRenderer.name}");
        }
    }

    public void StartTalking()
    {
        if (talkRoutine == null)
            talkRoutine = StartCoroutine(TalkRoutine());
        talking = true;
    }

    public void StopTalking()
    {
        talking = false;

        if (talkRoutine != null)
        {
            StopCoroutine(talkRoutine);
            talkRoutine = null;
        }

        StopAllCoroutines();

        foreach (var shape in shapeIndices)
        {
            faceRenderer.SetBlendShapeWeight(shape.Value, 0f);
        }
    }


    IEnumerator TalkRoutine()
    {
        while (true)
        {
            if (!talking)
            {
                yield return null;
                continue;
            }

            var viseme = visemeShapes[Random.Range(0, visemeShapes.Count)];
            currentShape = viseme.name;
            currentWeight = viseme.maxWeight;

            foreach (var shape in shapeIndices)
            {
                float target = (shape.Key == currentShape) ? currentWeight : 0f;
                StartCoroutine(BlendTo(shape.Value, target));
            }

            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator BlendTo(int index, float targetWeight)
    {
        float current = faceRenderer.GetBlendShapeWeight(index);
        while (!Mathf.Approximately(current, targetWeight))
        {
            current = Mathf.MoveTowards(current, targetWeight, blendSpeed * Time.deltaTime);
            faceRenderer.SetBlendShapeWeight(index, current);
            yield return null;
        }
    }
}
