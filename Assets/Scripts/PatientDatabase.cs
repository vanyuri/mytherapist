// ID: PDB_104
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public static class PatientDatabase
{
    public static List<Patient> GetPatients()
    {
        return new List<Patient>
        {
            new Patient
            {
                name = "Anna",
                issue = "Anxiety",
                personality = "Shy and polite",
                emotion = 90f,
                scriptedIntro = "Hi... I'm Anna. I'm 19 years old, and honestly, I don�t know what to expect from this. I just know I struggle with anxiety and it feels like it's getting harder to deal with every day.",
                promptContext = "You are Anna, a 19-year-old girl with social anxiety. You're talking to your school psychiatrist. Try to be open and respond gently, even if you're unsure."
            },
            new Patient
            {
                name = "Leo",
                issue = "Anger Management",
                personality = "Stubborn and blunt",
                emotion = 40f,
                scriptedIntro = "I�m Leo. I�m 17, and to be honest, I don�t see why I�m here. I was told to come, and now I�m stuck talking about feelings or whatever this is.",
                promptContext = "You are Leo, a 17-year-old student with anger issues. You're talking to your school psychiatrist. Be honest and blunt, but assume they mean well."
            },
            new Patient
            {
                name = "Mira",
                issue = "Perfectionism & Burnout",
                personality = "High-achiever, anxious",
                emotion = 40f,
                scriptedIntro = "My name is Mira, and I�m 18. I try to be perfect at everything, but lately, it feels like I can never get anything right, no matter how hard I work.",
                promptContext = "You are Mira, an overachieving 18-year-old student dealing with burnout. You're talking to your school psychiatrist. Be thoughtful and sensitive to how the messages make you feel."
            }
        };
    }
}

public class Patient
{
    public string name;
    public string issue;
    public string personality;
    public float emotion;
    public string scriptedIntro;
    public string promptContext;

    public string GetCombinedPrompt(string userMessage)
    {
        return $"{promptContext}\n\nPsychiatrist ({GameManager.Instance.doctorName}): {userMessage}";
    }
}
