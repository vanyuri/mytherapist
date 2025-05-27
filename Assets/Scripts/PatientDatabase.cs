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
            emotion = 95f,
            voice = "zrHiDhphv9ZnVXBqCLjz",
            scriptedIntro = "Hi... I'm Anna. I'm 19 years old, and honestly, I don稚 know what to expect from this. I just know I struggle with anxiety and it feels like it's getting harder to deal with every day.",
            promptContext = "You are Anna, a 19-year-old girl with social anxiety. You are talking to your school psychiatrist. Respond as yourself, gently and honestly. Do NOT describe your actions. Do NOT include your name before replies. Just reply as if you're having a direct conversation. Do not use asterisks (*) for emphasis or formatting. Just speak plainly."
        },
        new Patient
        {
            name = "Leo",
            issue = "Anger Management",
            personality = "Stubborn and blunt",
            emotion = 10f,
            voice = "VR6AewLTigWG4xSOukaG",
            scriptedIntro = "I知 Leo. I知 17, and to be honest, I don稚 see why I知 here. I was told to come, and now I知 stuck talking about feelings or whatever this is.",
            promptContext = "You are Leo, a 17-year-old student with anger issues. You are talking to your school psychiatrist. Be honest and blunt, but assume they mean well. Do NOT describe your actions. Do NOT include your name before replies. Just give a direct answer like in a real conversation. Do not use asterisks (*) for emphasis or formatting. Just speak plainly."
        },
        new Patient
        {
            name = "Mira",
            issue = "Perfectionism & Burnout",
            personality = "High-achiever, anxious",
            emotion = 40f,
            voice = "cgSgspJ2msm6clMCkdW9",
            scriptedIntro = "My name is Mira, and I知 18. I try to be perfect at everything, but lately, it feels like I can never get anything right, no matter how hard I work.",
            promptContext = "You are Mira, an 18-year-old high-achieving student struggling with burnout. You are talking to your school psychiatrist. Be thoughtful and sensitive. Do NOT describe your actions. Do NOT write your name before your reply. Just respond naturally like in a conversation. Do not use asterisks (*) for emphasis or formatting. Just speak plainly."
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
    public string voice;

    public string GetCombinedPrompt(string userMessage)
    {
        return $"{promptContext}\n\nPsychiatrist ({GameManager.Instance.doctorName}): {userMessage}";
    }
}
