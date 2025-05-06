//GMC_105
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Patient Management")]
    private List<Patient> allPatients;
    public int currentPatientIndex = 0;
    public Patient CurrentPatient => allPatients[currentPatientIndex];

    [Header("Doctor Info")]
    public string doctorName = "Yuri";
    public int doctorAge = 32;
    public string doctorSpecialty = "Clinical Psychology";

    [Header("UI")]
    public Slider emotionSlider; // 👈 Assign in the Inspector
    public Button nextPatientButton; // 👈 Assign in the Inspector

    [Header("AI Script Reference")]
    public UnityAndGeminiV3 aiScript; // 👈 Drag the AI script here in Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            allPatients = PatientDatabase.GetPatients();

            if (allPatients.Count > 0)
            {
                currentPatientIndex = 0; // 👈 Start with first patient (Anna)
                Debug.Log($"Starting with patient: {CurrentPatient.name}");

                UpdateEmotionSlider();
                UpdateNextButton();

                if (aiScript != null)
                {
                    aiScript.ShowCurrentPatientIntro(); // 👈 Show intro on start
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EvaluateEmotionFromTag(string emotionTag)
    {
        int delta = 0;

        if (emotionTag.Contains("Good"))
            delta = Random.Range(5, 12);
        else if (emotionTag.Contains("Bad"))
            delta = Random.Range(-10, 5);
        else
            delta = 0;

        CurrentPatient.emotion += delta;
        CurrentPatient.emotion = Mathf.Clamp(CurrentPatient.emotion, 0f, 100f);

        Debug.Log($"[{CurrentPatient.name}] Emotion Meter: {CurrentPatient.emotion} (Δ{delta})");

        UpdateEmotionSlider();
        UpdateNextButton();

        if (CurrentPatient.emotion >= 100f)
            Debug.Log($"{CurrentPatient.name} is fully satisfied!");
        else if (CurrentPatient.emotion <= 0f)
            Debug.Log($"{CurrentPatient.name} is disengaged.");
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

    public void MoveToNextPatient()
    {

        if (aiScript != null)
        {
            aiScript.ShowCurrentPatientIntro(); // 👈 THIS must run when switching
        }

        if (currentPatientIndex + 1 < allPatients.Count)
        {
            currentPatientIndex++;
            Debug.Log($"Moved to patient: {CurrentPatient.name}");

            UpdateEmotionSlider();
            UpdateNextButton();

            if (aiScript != null)
            {
                aiScript.ShowCurrentPatientIntro(); // 👈 Show new intro
            }
        }
        else
        {
            Debug.Log("No more patients.");
            nextPatientButton.interactable = false;
        }
    }
}