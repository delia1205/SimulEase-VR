using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using SimpleJSON;
using System.Linq;

public class ContinuousSpeechRecognizer : MonoBehaviour, IEmotionProvider
{
    public AudioSource audioSource;
    public TalkingAlina Alina;
    public GazeFollowingHUD gazeHUD;

    private string microphoneDevice;
    private AudioClip recordingClip;
    private int sampleRate = 16000;
    private int maxChunkSeconds = 5;
    private float silenceThreshold = 0.05f;
    private float silenceDuration = 1.5f;
    private bool isListening = false;
    private float silenceTimer = 0f;
    private int lastSamplePosition = 0;
    private List<string> recognizedChunks = new List<string>();
    private List<Message> conversationHistory = new List<Message>();
    private bool conversationOngoing = true;

    private ConfigData config;
    private string subscriptionKey;
    private string region;
    private string openAiKey;
    private string openAiEndpoint;
    private string deployment;
    private string apiVersion;
    private string humeApiKey;

    private float conversationDuration;
    private float elapsedTime = 0f;
    private Dictionary<string, int> emotionFrequency = new Dictionary<string, int>();

    private string[] sensitiveEmotions = new string[]
    {
        "annoyance", "anxiety", "awkwardness", "confusion",
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

            subscriptionKey = config.subscriptionKey;
            region = config.region;
            openAiKey = config.openAiKey;
            openAiEndpoint = config.openAiEndpoint;
            deployment = config.deployment;
            apiVersion = config.apiVersion;
            humeApiKey = config.humeApiKey;
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
    }

    void Start()
    {
        StartCoroutine(ListenForSpeech((message) =>
        {
            string fullMessage = message;

            conversationHistory.Clear();
            conversationHistory.Add(new Message
            {
                role = "system",
                content = "You are a friendly, relaxed person sitting in a café having a casual conversation with the user. Speak naturally and informally, like you're talking to a friend. Talk about music, travel, daily life, or anything the user brings up. Remember what the user says so you can refer back to it naturally. Don't mention you're an AI. "
            });
            conversationHistory.Add(new Message { role = "user", content = fullMessage });

            StartCoroutine(ConversationLoop());
        }));
    }

    IEnumerator ConversationLoop()
    {
        elapsedTime = 0f;

        while (conversationOngoing)
        {
            float loopStartTime = Time.time;

            string url = $"{openAiEndpoint}openai/deployments/{deployment}/chat/completions?api-version={apiVersion}";
            ChatRequest payload = new ChatRequest
            {
                messages = conversationHistory,
                temperature = 0.7f,
                max_tokens = 512,
                top_p = 1.0f,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            string requestBody = JsonUtility.ToJson(payload);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("api-key", openAiKey);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("OpenAI Error: " + request.error);
                yield break;
            }

            string json = request.downloadHandler.text;
            string responseText = ExtractResponseFromJson(json);
            conversationHistory.Add(new Message { role = "assistant", content = responseText });
            yield return StartCoroutine(SpeakText(responseText));

            elapsedTime += Time.time - loopStartTime;

            string durationStr = PlayerPrefs.GetString("SelectedDuration", "5");
            if (float.TryParse(durationStr, out float durationMinutes))
            {
                conversationDuration = durationMinutes * 60f;
            }
            else
            {
                conversationDuration = 300f;
            }

            if (conversationDuration - elapsedTime <= 10f)
            {
                string farewell = "Oh, looks like this is all the time we have today! It's been great catching up with you. Let's talk again soon!";
                conversationHistory.Add(new Message { role = "assistant", content = farewell });
                yield return StartCoroutine(SpeakText(farewell));

                conversationOngoing = false;
                yield break;
            }

            yield return StartCoroutine(WaitForNextUserInput());
        }
    }


    IEnumerator WaitForNextUserInput()
    {
        yield return StartCoroutine(ListenForSpeech((userMessage) =>
        {
            conversationHistory.Add(new Message { role = "user", content = userMessage });
        }));
    }

    IEnumerator ListenForSpeech(System.Action<string> onComplete)
    {
        isListening = true;
        silenceTimer = 0f;
        lastSamplePosition = 0;
        recognizedChunks.Clear();

        recordingClip = Microphone.Start(microphoneDevice, true, 60, sampleRate);
        yield return new WaitUntil(() => Microphone.GetPosition(microphoneDevice) > 0);

        float totalRecordingTime = 0f;

        while (isListening)
        {
            int currentPosition = Microphone.GetPosition(microphoneDevice);
            int sampleCount = (currentPosition >= lastSamplePosition)
                ? currentPosition - lastSamplePosition
                : (recordingClip.samples - lastSamplePosition + currentPosition);

            if (sampleCount < sampleRate * maxChunkSeconds)
            {
                totalRecordingTime += Time.deltaTime;
                yield return null;
                continue;
            }

            float[] samples = new float[sampleRate * maxChunkSeconds];
            recordingClip.GetData(samples, lastSamplePosition);
            lastSamplePosition = (lastSamplePosition + sampleRate * maxChunkSeconds) % recordingClip.samples;

            float maxVolume = 0f;
            foreach (float sample in samples)
                maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample));

            if (maxVolume > silenceThreshold)
            {
                silenceTimer = 0f;
            }
            else
            {
                silenceTimer += maxChunkSeconds;
            }

            if (maxVolume > silenceThreshold)
            {
                byte[] wavData = WavUtility.FromAudioClip(CreateClip(samples));
                yield return StartCoroutine(SendToAzure(wavData));
                yield return StartCoroutine(SendToHume(wavData));
            }

            if (silenceTimer >= silenceDuration)
            {
                isListening = false;
                Microphone.End(microphoneDevice);
                string message = string.Join(" ", recognizedChunks);
                onComplete?.Invoke(message);
                yield break;
            }
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
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Hume job submitted successfully: " + request.downloadHandler.text);
            string json = request.downloadHandler.text;
            string jobId = ExtractJobId(json);

            if (!string.IsNullOrEmpty(jobId))
            {
                StartCoroutine(GetHumePrediction(jobId));
            }
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


    IEnumerator SpeakText(string text)
    {
        string ttsEndpoint = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";
        string tokenUrl = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        UnityWebRequest tokenRequest = UnityWebRequest.PostWwwForm(tokenUrl, "");
        tokenRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        yield return tokenRequest.SendWebRequest();

        if (tokenRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS Token Error: " + tokenRequest.error);
            yield break;
        }

        string accessToken = tokenRequest.downloadHandler.text;
        string ssml = $@"
        <speak version='1.0' xml:lang='en-US'>
          <voice xml:lang='en-US' xml:gender='Female' name='en-US-JennyNeural'>
            <prosody rate='+20%'>
            {text}
            </prosody>
          </voice>
        </speak>";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(ssml);

        UnityWebRequest ttsRequest = new UnityWebRequest(ttsEndpoint, "POST");
        ttsRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        ttsRequest.downloadHandler = new DownloadHandlerAudioClip(ttsEndpoint, AudioType.WAV);
        ttsRequest.SetRequestHeader("Content-Type", "application/ssml+xml");
        ttsRequest.SetRequestHeader("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");
        ttsRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        ttsRequest.SetRequestHeader("User-Agent", "UnityTTSClient");
        yield return ttsRequest.SendWebRequest();

        if (ttsRequest.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(ttsRequest);
            audioSource.clip = clip;

            if (Alina != null)
                Alina.StartTalking();
            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying);
            if (Alina != null)
                Alina.StopTalking();
        }
        else
        {
            Debug.LogError("TTS Request Error: " + ttsRequest.error);
        }
    }

    AudioClip CreateClip(float[] samples)
    {
        AudioClip clip = AudioClip.Create("Chunk", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    IEnumerator SendToAzure(byte[] wavData)
    {
        string tokenUrl = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        UnityWebRequest tokenRequest = UnityWebRequest.PostWwwForm(tokenUrl, "");
        tokenRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        yield return tokenRequest.SendWebRequest();

        if (tokenRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Token Error: " + tokenRequest.error);
            yield break;
        }
        string accessToken = tokenRequest.downloadHandler.text;

        string speechUrl = $"https://{region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US";
        UnityWebRequest speechRequest = new UnityWebRequest(speechUrl, "POST");
        speechRequest.uploadHandler = new UploadHandlerRaw(wavData);
        speechRequest.downloadHandler = new DownloadHandlerBuffer();
        speechRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        speechRequest.SetRequestHeader("Content-Type", "audio/wav; codecs=audio/pcm; samplerate=16000");
        yield return speechRequest.SendWebRequest();

        if (speechRequest.result == UnityWebRequest.Result.Success)
        {
            string json = speechRequest.downloadHandler.text;
            string result = ExtractTextFromJson(json);
            if (!string.IsNullOrEmpty(result))
            {
                recognizedChunks.Add(result);
            }
        }
        else
        {
            Debug.LogError("Azure Error: " + speechRequest.error);
        }
    }

    string ExtractTextFromJson(string json)
    {
        string marker = "\"DisplayText\":\"";
        int start = json.IndexOf(marker);
        if (start < 0) return null;

        start += marker.Length;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    string ExtractResponseFromJson(string json)
    {
        string marker = "\"content\":\"";
        int start = json.IndexOf(marker);
        if (start < 0) return "No response from AI.";

        start += marker.Length;
        int end = json.IndexOf("\"", start);
        while (end > 0 && json[end - 1] == '\\') 
        {
            end = json.IndexOf("\"", end + 1);
        }

        return json.Substring(start, end - start).Replace("\\n", "\n").Replace("\\\"", "\"");
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

[System.Serializable]
public class ConfigData
{
    public string subscriptionKey;
    public string region;
    public string openAiKey;
    public string openAiEndpoint;
    public string deployment;
    public string apiVersion;
    public string humeApiKey;
}

[System.Serializable]
public class ChatRequest
{
    public List<Message> messages;
    public float temperature;
    public int max_tokens;
    public float top_p;
    public float frequency_penalty;
    public float presence_penalty;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatResponse
{
    public List<Choice> choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

