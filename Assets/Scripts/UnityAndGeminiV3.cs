// ID: UGC_104
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
        string userMessage = inputField.text.Trim();

        if (string.IsNullOrEmpty(userMessage))
        {
            Debug.Log("[UGC_104] Input is empty, skipping send.");
            return;
        }

        Patient currentPatient = GameManager.Instance.CurrentPatient;
        string combinedPrompt = currentPatient.GetCombinedPrompt(userMessage);

        StartCoroutine(SendChatRequest(combinedPrompt, userMessage));

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private IEnumerator SendChatRequest(string prompt, string playerInput)
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
            Debug.LogError("[UGC_104] API Error: " + www.error);
        }
        else
        {
            TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
            string reply = response?.candidates?[0]?.content?.parts?[0]?.text?.Trim();


            if (!string.IsNullOrEmpty(reply))
            {
                Debug.Log("[FULL] Full AI Response:\n" + reply);
                outputText.text = reply;

                GameManager.Instance.UpdatePatientReply(reply);

                chatHistory.Add(new TextContent
                {
                    role = "model",
                    parts = new[] { new TextPart { text = reply } }
                });

                // Get current patient context
                Patient currentPatient = GameManager.Instance.CurrentPatient;

                // Evaluate emotion in separate script
                EmotionEvaluator.Instance.EvaluateEmotion(playerInput, reply, currentPatient.promptContext, (resultTag) =>
                {
                    Debug.Log($"[EMOTION RESULT] Evaluation returned: {resultTag}");
                    GameManager.Instance.EvaluateEmotionFromTag(resultTag);
                });
            }
            else
            {
                Debug.LogWarning("[UGC_104] No valid response from Gemini.");
            }
        }
    }

    public void ShowCurrentPatientIntro()
    {
        Patient currentPatient = GameManager.Instance.CurrentPatient;

        if (!string.IsNullOrEmpty(currentPatient.scriptedIntro))
        {
            chatHistory.Clear();
            outputText.text = currentPatient.scriptedIntro;

            Debug.Log($"[AI] Showing intro for {currentPatient.name}: {currentPatient.scriptedIntro}");

            chatHistory.Add(new TextContent
            {
                role = "model",
                parts = new[] { new TextPart { text = currentPatient.scriptedIntro } }
            });
        }
        else
        {
            Debug.LogWarning($"[AI] No intro found for: {currentPatient.name}");
        }
    }
}
