using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FlutterUnityIntegration;
using UnityEditor;
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
        SendMessageToFlutter("quit");
    }

    public void Return()
    {
        SceneManager.LoadScene("Menu");
    }
    
    private void SendMessageToFlutter(string message)
    {
        UnityMessageManager.Instance.SendMessageToFlutter(message);
    }
}
