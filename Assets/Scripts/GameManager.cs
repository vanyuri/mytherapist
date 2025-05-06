// GMC_108
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public Slider emotionSlider; // 👈 Assign in Inspector
    public Button nextPatientButton; // 👈 Assign in Inspector
    public TextMeshProUGUI patientNameText; // 👈 Assign in Inspector
    public TextMeshProUGUI patientReplyText; // 👈 Assign in Inspector
    public List<GameObject> patientImages; // 👈 Assign images in order of patients

    [Header("AI Script Reference")]
    public UnityAndGeminiV3 aiScript; // 👈 Assign in Inspector

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

                UpdateEmotionSlider();
                UpdateNextButton();
                UpdatePatientNameUI();
                UpdatePatientReply(""); // Start empty
                UpdatePatientImage();

                if (aiScript != null)
                {
                    aiScript.ShowCurrentPatientIntro();
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

            UpdateEmotionSlider();
            UpdateNextButton();
            UpdatePatientNameUI();
            UpdatePatientReply(CurrentPatient.scriptedIntro);
            UpdatePatientImage();

            if (aiScript != null)
            {
                aiScript.ShowCurrentPatientIntro();
            }
        }
        else
        {
            Debug.Log("No more patients.");
            nextPatientButton.interactable = false;
        }
    }
}
