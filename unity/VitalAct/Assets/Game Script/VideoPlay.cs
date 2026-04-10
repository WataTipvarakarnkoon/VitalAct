using UnityEngine;
using UnityEngine.Video;

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
        if (state == GameManager.GameState.SetUp)
        {
            StartCoroutine(RestartVideo());
        }
        else
        {
            videoPlayer.Pause();
        }
    }

    System.Collections.IEnumerator RestartVideo()
    {
        videoPlayer.Stop();
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();
    }
}