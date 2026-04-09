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
            videoPlayer.Stop();
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }
    }
}