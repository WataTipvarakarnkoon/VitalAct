using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string[] scenes = {"Room", "Road"};
    public void Play()
    {
        string picked = scenes[Random.Range(0, scenes.Length)];
        SceneManager.LoadScene(picked);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
