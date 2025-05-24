using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ElevenLabsTTS : MonoBehaviour
{
    /*
    Example voices:
    zrHiDhphv9ZnVXBqCLjz
    VR6AewLTigWG4xSOukaG
    */

    public string apiKey = "sk_1d339f04149df7eccb72c72491154b89e5240d972fb54c1a"; // Replace with your actual API key
    public string voiceId; // Default voice ID
    public AudioSource audioSource;

    private string baseUrl = "https://api.elevenlabs.io/v1/text-to-speech/";

    public static ElevenLabsTTS Instance;


    void Start()
    {
        SetVoiceId("VR6AewLTigWG4xSOukaG");
     

        Speak("Hello, this is a test from ElevenLabs.");
        

    }

    // Call this method to update the voice ID before speaking
    public void SetVoiceId(string newVoiceId)
    {
        voiceId = newVoiceId;
        Debug.Log("Voice ID changed to: " + voiceId);
    }

    public void Speak(string message)
    {
        Debug.Log("Speak() called with message: " + message);
        StartCoroutine(GenerateAndPlayAudio(message));
    }

    IEnumerator GenerateAndPlayAudio(string text)
    {
        string url = baseUrl + voiceId;
        Debug.Log("Using Voice ID: " + voiceId);
        Debug.Log("Sending request to: " + url);

        // Prepare JSON body
        ElevenRequest requestData = new ElevenRequest { text = text };
        string jsonBody = JsonUtility.ToJson(requestData);
        Debug.Log("Request JSON: " + jsonBody);

        // Setup UnityWebRequest
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("xi-api-key", apiKey);
        www.SetRequestHeader("Accept", "audio/mpeg");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS Error: " + www.error);
            Debug.LogError("Response code: " + www.responseCode);
            Debug.LogError("Response text: " + www.downloadHandler.text);
            yield break;
        }

        Debug.Log("TTS audio response received. Size: " + www.downloadHandler.data.Length + " bytes");

        byte[] audioData = www.downloadHandler.data;

        // Save to temp file
        string tempPath = Path.Combine(Application.persistentDataPath, "tempAudio.mp3");
        File.WriteAllBytes(tempPath, audioData);
        Debug.Log("Audio written to: " + tempPath);

        using (UnityWebRequest audioLoader = UnityWebRequestMultimedia.GetAudioClip("file://" + tempPath, AudioType.MPEG))
        {
            Debug.Log("Loading audio from saved file...");
            yield return audioLoader.SendWebRequest();

            if (audioLoader.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio Load Error: " + audioLoader.error);
            }
            else
            {
                Debug.Log("Audio loaded successfully.");
                AudioClip clip = DownloadHandlerAudioClip.GetContent(audioLoader);
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log("Audio is playing.");
            }
        }
    }

    [System.Serializable]
    public class ElevenRequest
    {
        public string text;
        public string model_id = "eleven_monolingual_v1";
        public VoiceSettings voice_settings = new VoiceSettings();
    }

    [System.Serializable]
    public class VoiceSettings
    {
        public float stability = 0.5f;
        public float similarity_boost = 0.5f;
    }
}
