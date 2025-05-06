// ID: EMO_101
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[System.Serializable]
public class EmotionResult
{
    public string emotion;
}

public class EmotionEvaluator : MonoBehaviour
{
    public static EmotionEvaluator Instance;

    [Header("Configuration")]
    public TextAsset jsonApi;
    private string apiKey;
    private string endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        apiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text).key;
    }

    public void EvaluateEmotion(string playerMessage, string aiResponse, string patientPrompt, Action<string> callback)
    {
        string emotionPrompt = $"You are an emotion evaluator in a therapy simulation. The patient has the following background:\n{patientPrompt}\n\n" +
                               $"Player said: \"{playerMessage}\"\nPatient replied: \"{aiResponse}\"\n\n" +
                               "Evaluate the emotional impact of the player's message based on the patient's context and the reply. Respond with one word only: Good, Bad, or None.";

        StartCoroutine(SendEmotionRequest(emotionPrompt, callback));
    }

    private IEnumerator SendEmotionRequest(string prompt, Action<string> callback)
    {
        string url = $"{endpoint}?key={apiKey}";

        var content = new TextContent
        {
            role = "user",
            parts = new[] { new TextPart { text = prompt } }
        };

        ChatRequest request = new ChatRequest { contents = new[] { content } };
        string json = JsonUtility.ToJson(request);

        using UnityWebRequest www = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[EMO_101] Emotion API Error: " + www.error);
            callback?.Invoke("None");
        }
        else
        {
            TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
            string rawText = response?.candidates?[0]?.content?.parts?[0]?.text?.Trim();

            Debug.Log("[EMO_101] Emotion Evaluation Raw Response: " + rawText);
            string result = ParseEmotion(rawText);
            callback?.Invoke(result);
        }
    }

    private string ParseEmotion(string text)
    {
        if (text.Contains("Good", StringComparison.OrdinalIgnoreCase)) return "Good";
        if (text.Contains("Bad", StringComparison.OrdinalIgnoreCase)) return "Bad";
        return "None";
    }
}
