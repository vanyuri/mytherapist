// ID: UGC_103
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UnityAndGeminiKey { public string key; }
[System.Serializable]
public class TextPart { public string text; }
[System.Serializable]
public class TextContent { public string role; public TextPart[] parts; }
[System.Serializable]
public class ChatRequest { public TextContent[] contents; }
[System.Serializable]
public class TextCandidate { public TextContent content; }
[System.Serializable]
public class TextResponse { public TextCandidate[] candidates; }

public class UnityAndGeminiV3 : MonoBehaviour
{
    [Header("Configuration")]
    public TextAsset jsonApi;
    private string apiKey;
    private string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    [Header("UI Elements")]
    public TMP_InputField inputField;
    public TMP_Text outputText;

    private List<TextContent> chatHistory = new();

    void Start()
    {
        UnityAndGeminiKey keyData = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = keyData.key;

        // Use the patient from GameManager
        if (GameManager.Instance.CurrentPatient != null)
        {
            Patient currentPatient = GameManager.Instance.CurrentPatient;
            if (!string.IsNullOrEmpty(currentPatient.scriptedIntro))
            {
                outputText.text = currentPatient.scriptedIntro;
            }
        }
        else
        {
            Debug.LogError("No patient assigned!");
            return;
        }
    }

    public void SendChat()
    {

        string userMessage = inputField.text;
        Patient currentPatient = GameManager.Instance.CurrentPatient;
        //string emotionContext = GameManager.Instance.GetEmotionContext();
        //string combinedPrompt = $"{currentPatient.GetCombinedPrompt(userMessage)}\n[EmotionState: {emotionContext}]";
        string combinedPrompt = currentPatient.GetCombinedPrompt(userMessage);

        StartCoroutine(SendChatRequest(combinedPrompt));
       // Debug.Log("[DEBUG] Final AI Prompt:\n" + combinedPrompt);

    }

    private IEnumerator SendChatRequest(string prompt)
    {
        string url = $"{apiEndpoint}?key={apiKey}";

        var userContent = new TextContent
        {
            role = "user",
            parts = new[] { new TextPart { text = prompt } }
        };

        chatHistory.Add(userContent);
        ChatRequest request = new ChatRequest { contents = chatHistory.ToArray() };
        string jsonData = JsonUtility.ToJson(request);

        using UnityWebRequest www = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
            string reply = response?.candidates?[0]?.content?.parts?[0]?.text;

            if (!string.IsNullOrEmpty(reply))
            {
                // Extract [EmotionScore:+X]
                
                string cleanReply = reply;
                var emotionMatch = System.Text.RegularExpressions.Regex.Match(reply, @"\[Emotion:(Good|Bad|None)\]");
                if (emotionMatch.Success)
                {
                    string tag = emotionMatch.Groups[1].Value;
                    GameManager.Instance.EvaluateEmotionFromTag(tag);
                    cleanReply = reply.Replace(emotionMatch.Value, "").Trim();
                }

                outputText.text = cleanReply;

                chatHistory.Add(new TextContent
                {
                    role = "model",
                    parts = new[] { new TextPart { text = cleanReply } }
                });

            }
            else
            {
                Debug.Log("No valid response.");
            }
        }
    }

}
