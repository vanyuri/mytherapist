// ID: GMC_102
using UnityEngine;
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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            allPatients = PatientDatabase.GetPatients();
            if (allPatients.Count > 0)
            {
                currentPatientIndex = Random.Range(0, allPatients.Count);
                Debug.Log($"Selected patient: {CurrentPatient.name}");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPatientByIndex(int index)
    {
        if (index >= 0 && index < allPatients.Count)
        {
            currentPatientIndex = index;
            Debug.Log($"Switched to patient: {CurrentPatient.name}");
        }
    }

    public void EvaluateEmotionFromTag(string emotionTag)
    {
        int delta = 0;
        Debug.Log($"{emotionTag} reply");

        if (emotionTag.Contains("Good"))
            delta = Random.Range(5, 12); // +1 to +10
        else if (emotionTag.Contains("Bad"))
            delta = Random.Range(-10, 0); // -10 to -1
        else
            delta = 0; // No change

        CurrentPatient.emotion += delta;
        CurrentPatient.emotion = Mathf.Clamp(CurrentPatient.emotion, 0f, 100f);

        Debug.Log($"[{CurrentPatient.name}] Emotion Meter: {CurrentPatient.emotion} (Δ{delta})");

        if (CurrentPatient.emotion >= 100f)
            Debug.Log($"{CurrentPatient.name} is fully satisfied!");
        else if (CurrentPatient.emotion <= 0f)
            Debug.Log($"{CurrentPatient.name} is disengaged.");
    }


   /* public string GetEmotionContext()
    {
        if (CurrentPatient.emotion < 30f)
            return "The student feels distant or uncomfortable. Respond cautiously and gently.";
        else if (CurrentPatient.emotion < 70f)
            return "The student is neutral. Respond normally and respectfully.";
        else
            return "The student feels open and trusting. You can be warm, encouraging, and supportive.";
    }
   */
}
