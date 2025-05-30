﻿// GMC_108
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private float currentEmotion;
    public TextToSpeech elevenLabsTTS; // Drag your ElevenLabsTTS GameObject here
   


    [Header("Patient Management")]
    private List<Patient> allPatients;
    public int currentPatientIndex;
    public Patient CurrentPatient => allPatients[currentPatientIndex];

    [Header("Doctor Info")]
    public string doctorName = "Yuri";
    public int doctorAge = 32;
    public string doctorSpecialty = "Clinical Psychology";

    [Header("UI")]
    public Slider emotionSlider; 
    public Button nextPatientButton; 
    public TextMeshProUGUI patientNameText;
    public TextMeshProUGUI patientReplyText; 
    public List<GameObject> patientImages; 
    public GameObject menuPanel;
    public GameObject gameOverPanel;
    public GameObject sessionCompletePanel;


    void Awake()
    {

     
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            allPatients = PatientDatabase.GetPatients();

            if (allPatients.Count > 0)
            {
                currentPatientIndex = 0;
                Debug.Log($"Starting with patient: {CurrentPatient.name}");

                currentEmotion = CurrentPatient.emotion;
                UpdateEmotionSlider();
                UpdateNextButton();
                UpdatePatientNameUI();
                UpdatePatientReply(""); 
                UpdatePatientImage();
            }
        }
        else
        {
           // Destroy(gameObject);
        }
    }

    public void EvaluateEmotionFromTag(string emotionTag)
    {
        int delta;

        if(CurrentPatient.emotion < 100)
        {
       
            if (emotionTag.Contains("Good"))
               delta = Random.Range(5, 12);
       
            else if (emotionTag.Contains("Bad"))
       
               delta = Random.Range(-10, -5);
            else
               delta = 0;

            CurrentPatient.emotion += delta;
            CurrentPatient.emotion = Mathf.Clamp(CurrentPatient.emotion, 0f, 100f);
       
            Debug.Log($"[{CurrentPatient.name}] Emotion Meter: {CurrentPatient.emotion} (Δ{delta})");

            UpdateEmotionSlider();
            UpdateNextButton();

            if (CurrentPatient.emotion >= 100f)
                 {
                     ShowSessionCompletePanel();
                 }
       
            else if (CurrentPatient.emotion <= 0f) 
                {
                     ShowGameOverPanel(); 
                }
        }

    }

    private void UpdateEmotionSlider()
    {
        if (emotionSlider != null)
            emotionSlider.value = CurrentPatient.emotion;
    }

    private void UpdateNextButton()
    {
        if (nextPatientButton != null)
            nextPatientButton.interactable = CurrentPatient.emotion >= 100f;
    }

    private void UpdatePatientNameUI()
    {
        if (patientNameText != null)
            patientNameText.text = CurrentPatient.name;
    }

    public void UpdatePatientReply(string reply)
    {
        if (patientReplyText != null)
            patientReplyText.text = reply;
    }

    private void UpdatePatientImage()
    {
        for (int i = 0; i < patientImages.Count; i++)
        {
            if (patientImages[i] != null)
                patientImages[i].SetActive(i == currentPatientIndex);
        }
    }

    public void MoveToNextPatient()
    {
        if (currentPatientIndex + 1 < allPatients.Count)
        {
            currentPatientIndex++;
            Debug.Log($"Moved to patient: {CurrentPatient.name}");

            currentEmotion = CurrentPatient.emotion;

            
            UpdateEmotionSlider();
            UpdateNextButton();
            UpdatePatientNameUI();
            UpdatePatientReply(CurrentPatient.scriptedIntro);
            UpdatePatientImage();
           // elevenLabsTTS.SetVoiceId(CurrentPatient.voice);
           // elevenLabsTTS.Speak(CurrentPatient.scriptedIntro);
            CloseSessionCompletePanel();

        }
        else
        {
            Debug.Log("No more patients.");
            nextPatientButton.interactable = false;
        }
    }

    public void RestartCurrentPatient()
    {
        Debug.Log($"Restarting patient: {CurrentPatient.name}");

        // Reset emotion meter
        CurrentPatient.emotion = currentEmotion;
       


        // Reset UI
        UpdateEmotionSlider();
        UpdateNextButton();
        UpdatePatientNameUI();
        UpdatePatientImage();

        // Reset conversation
        UnityAndGeminiV3 chatHandler = FindObjectOfType<UnityAndGeminiV3>();
        if (chatHandler != null)
        {
            chatHandler.ResetConversation();
        }

        // Show scripted intro again
        UpdatePatientReply(CurrentPatient.scriptedIntro);
       // elevenLabsTTS.Speak(CurrentPatient.scriptedIntro);

        menuPanel.SetActive(false);
        CloseGameOverPanel();
    }

    public void ToggleMenu()
    {
        if (menuPanel != null)
        {
            bool isActive = menuPanel.activeSelf;
            menuPanel.SetActive(!isActive);
        }
    }

    public void HideMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");

        // This will quit the game when built as an application
        Application.Quit();

        // For editor testing
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    // Close Game Over Panel
    public void CloseGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    // Show Session Complete Panel
    public void ShowSessionCompletePanel()
    {
        sessionCompletePanel.SetActive(true);
    }

    // Close Session Complete Panel
    public void CloseSessionCompletePanel()
    {
        sessionCompletePanel.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0); 
    }
}
