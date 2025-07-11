using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
using System.Linq;

public class HumeEmotionListener : MonoBehaviour, IEmotionProvider
{
    public int sampleRate = 16000;
    public int chunkSeconds = 5;
    public GazeFollowingHUD gazeHUD;

    private string microphoneDevice;
    private AudioClip recordingClip;
    private ConfigData config;
    private string humeApiKey;

    private Dictionary<string, int> emotionFrequency = new Dictionary<string, int>();

    private string[] sensitiveEmotions = new string[]
    {
        "anger", "annoyance", "anxiety", "awkwardness", "confusion",
        "disappointment", "distress", "doubt",
        "embarrassment", "fear", "surprise (negative)"
    };

    private void Awake()
    {
        string path = Path.Combine(Application.dataPath, "Scripts", "config.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            config = JsonUtility.FromJson<ConfigData>(json);
            humeApiKey = config.humeApiKey;
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
    }

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
            recordingClip = Microphone.Start(microphoneDevice, true, 60, sampleRate);
            StartCoroutine(LoopListenAndSend());
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    IEnumerator LoopListenAndSend()
    {
        yield return new WaitUntil(() => Microphone.GetPosition(microphoneDevice) > 0);

        while (true)
        {
            yield return new WaitForSeconds(chunkSeconds);

            int currentPosition = Microphone.GetPosition(microphoneDevice);
            int chunkSamples = sampleRate * chunkSeconds;

            float[] samples = new float[chunkSamples];
            if (currentPosition >= chunkSamples)
            {
                recordingClip.GetData(samples, currentPosition - chunkSamples);
            }
            else
            {
                int wrapAroundSamples = chunkSamples - currentPosition;
                float[] wrappedSamples = new float[chunkSamples];
                recordingClip.GetData(wrappedSamples, recordingClip.samples - wrapAroundSamples);
                recordingClip.GetData(wrappedSamples, 0);
                samples = wrappedSamples;
            }

            AudioClip chunkClip = AudioClip.Create("Chunk", chunkSamples, 1, sampleRate, false);
            chunkClip.SetData(samples, 0);
            byte[] wavData = WavUtility.FromAudioClip(chunkClip);

            StartCoroutine(SendToHume(wavData));
        }
    }

    IEnumerator SendToHume(byte[] audioData)
    {
        string url = "https://api.hume.ai/v0/batch/jobs";

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("json", "{\"models\": {\"prosody\": {}}}"),
            new MultipartFormFileSection("file", audioData, "chunk.wav", "audio/wav")
        };

        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.SetRequestHeader("X-Hume-Api-Key", humeApiKey);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Hume Request Error: " + request.responseCode + " - " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            string jobId = ExtractJobId(json);

            if (!string.IsNullOrEmpty(jobId))
                StartCoroutine(GetHumePrediction(jobId));
        }
    }

    IEnumerator GetHumePrediction(string jobId)
    {
        string url = $"https://api.hume.ai/v0/batch/jobs/{jobId}/predictions";
        yield return new WaitForSeconds(5f); 

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Hume-Api-Key", humeApiKey);
        request.SetRequestHeader("accept", "application/json; charset=utf-8");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Prediction Retrieval Error: " + request.error);
        }
        else
        {
            string predictionJson = request.downloadHandler.text;
            List<string> detectedEmotions = ExtractRelevantEmotions(predictionJson);
            if (detectedEmotions.Count == 0)
            {
                Debug.Log("No strong emotions detected.");
            }
            else
            {
                Debug.Log("Relevant emotions: " + string.Join(", ", detectedEmotions));
                foreach (string emotion in detectedEmotions)
                {
                    if (emotionFrequency.ContainsKey(emotion))
                        emotionFrequency[emotion]++;
                    else
                        emotionFrequency[emotion] = 1;
                }

                string triggered = detectedEmotions.FirstOrDefault(e => sensitiveEmotions.Contains(e));
                if (!string.IsNullOrEmpty(triggered))
                {
                    string message = $"We're sensing some {triggered} in your voice. Remember you can always take a deep breath and relax, or pause the experience.";
                    gazeHUD.ShowMessage(message);
                }
            }
        }
    }

    string ExtractJobId(string json)
    {
        string marker = "\"job_id\":\"";
        int start = json.IndexOf(marker);
        if (start < 0) return null;

        start += marker.Length;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    List<string> ExtractRelevantEmotions(string json, float threshold = 0.20f, int max = 3)
    {
        var root = JSON.Parse(json);
        var emotions = root[0]["results"]["predictions"][0]["models"]["prosody"]["grouped_predictions"][0]["predictions"][0]["emotions"];

        List<(string name, float score)> top = new();

        foreach (JSONNode emotion in emotions.AsArray)
        {
            string name = emotion["name"];
            float score = emotion["score"].AsFloat;
            if (score >= threshold)
                top.Add((name, score));
        }

        top.Sort((a, b) => b.score.CompareTo(a.score));

        return top.Take(max).Select(e => e.name.ToLower()).ToList();
    }

    public Dictionary<string, int> GetEmotionFrequency()
    {
        return new Dictionary<string, int>(emotionFrequency);
    }

}
