using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;

public class VideoSelector : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button[] videoButtons;
    public string[] videoFileNames;

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
        if (index < 0 || index >= videoFileNames.Length) return;

        string path = Path.Combine(Application.streamingAssetsPath, "Video", videoFileNames[index]);
        path = path.Replace("\\", "/");

        videoPlayer.url = path;
        videoPlayer.Play();
    }
}