using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using FlutterUnityIntegration;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void OnEnable()
    {
        GameManager.OnStateChanged += HandleState;
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= HandleState;
    }

    void HandleState(GameManager.GameState state)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (state == GameManager.GameState.SetUp)
            UnityMessageManager.Instance.SendMessageToFlutter("video:restart");
        else
            UnityMessageManager.Instance.SendMessageToFlutter("video:pause");
#else
        if (state == GameManager.GameState.SetUp)
            StartCoroutine(RestartVideo());
        else
            videoPlayer.Pause();
#endif
    }

    IEnumerator RestartVideo()
    {
        videoPlayer.Stop();
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;
        videoPlayer.Play();
    }
}
