using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
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
}
