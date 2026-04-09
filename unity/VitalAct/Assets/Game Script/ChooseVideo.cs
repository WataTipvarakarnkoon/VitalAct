using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoSelector : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button[] videoButtons;
    public VideoClip[] videoClips;

    void Start()
    {
        for (int i = 0; i < videoButtons.Length; i++)
        {
            int index = i;
            videoButtons[i].onClick.AddListener(() => PlayVideo(index));
        }
    }

    void PlayVideo(int index)
    {
        if (index < 0 || index >= videoClips.Length) return;

        videoPlayer.clip = videoClips[index];
        videoPlayer.Play();
    }
}