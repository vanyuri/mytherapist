using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class VoiceSettings
{
    public float stability;
    public float similarity_boost;
    public float style;
    public bool use_speaker_boost;
}

[Serializable]
public class PronunciationDictionaryLocator
{
    public string pronunciation_dictionary_id;
    public string version_id;
}

[Serializable]
public class TTSData
{
    public string text;
    public string model_id;
    public PronunciationDictionaryLocator[] pronunciation_dictionary_locators;
    public VoiceSettings voice_settings;
}

[Serializable]
public class ElevenLabsConfig
{
    public string apiKey = "sk_1d339f04149df7eccb72c72491154b89e5240d972fb54c1a";
    public string voiceId = "zrHiDhphv9ZnVXBqCLjz";
    public string ttsUrl = "https://api.elevenlabs.io/v1/text-to-speech/{0}/stream";
}

public class ELabs : MonoBehaviour
{
    public ElevenLabsConfig config;
    public AudioSource audioSource;
    public string text = "oh why hello there how are you? nice to meet you i'm van";

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogError("Text is empty or null.");
            return;
        }
        StartCoroutine(GenerateAndStreamAudio(text));
        StartCoroutine(DownloadAndPlayAudio(text));
    }

    private IEnumerator DownloadAndPlayAudio(string text)
    {
        yield return StartCoroutine(DownloadAudio(text, audioClip =>
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to download or decode audio clip.");
            }
        }));
    }

    private IEnumerator GenerateAndStreamAudio(string text)
    {
        string url = string.Format(config.ttsUrl, config.voiceId);

        TTSData ttsData = new TTSData
        {
            text = text.Trim(),
            model_id = "eleven_multilingual_v2",
            pronunciation_dictionary_locators = new PronunciationDictionaryLocator[] { /* Populate if needed */ },
            voice_settings = new VoiceSettings
            {
                stability = 0.5f,
                similarity_boost = 0.8f,
                style = 1.0f, // Adjust according to needs
                use_speaker_boost = true
            }
        };

        string jsonData = JsonUtility.ToJson(ttsData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", config.apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                yield break;
            }

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to download or decode audio clip.");
            }
        }
    }

    public IEnumerator DownloadAudio(string text, Action<AudioClip> onDownloaded)
    {
        string url = string.Format(config.ttsUrl, config.voiceId);

        TTSData ttsData = new TTSData
        {
            text = text.Trim(),
            model_id = "eleven_multilingual_v2",
            pronunciation_dictionary_locators = new PronunciationDictionaryLocator[] { /* Populate if needed */ },
            voice_settings = new VoiceSettings
            {
                stability = 0.5f,
                similarity_boost = 0.8f,
                style = 1.0f, // Adjust according to needs
                use_speaker_boost = true
            }
        };

        string jsonData = JsonUtility.ToJson(ttsData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", config.apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                onDownloaded?.Invoke(null);
                yield break;
            }
            Debug.Log("Sending request to: " + url);
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            if (audioClip != null)
            {
                onDownloaded?.Invoke(audioClip);
                Debug.Log("Download");
            }
            else
            {
                Debug.LogError("Failed to download or decode audio clip.");
                onDownloaded?.Invoke(null);
            }
        }
    }
}