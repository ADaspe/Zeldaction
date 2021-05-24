using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AXD_MainMenuManager : MonoBehaviour
{
    public string GameScene;

    public void Play()
    {
        SceneManager.LoadScene(GameScene);
    }

    public void Quit()
    {
        Debug.Log("Bye bye");
        Application.Quit();
    }
}
