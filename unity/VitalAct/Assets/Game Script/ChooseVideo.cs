using UnityEngine;
using UnityEngine.UI;
using FlutterUnityIntegration;

#if UNITY_EDITOR || !UNITY_ANDROID
using UnityEngine.Video;
using System.IO;
using System.Collections;
#endif

public class VideoSelector : MonoBehaviour
{
    public Button[] videoButtons;
    public string[] videoFileNames;

#if UNITY_EDITOR || !UNITY_ANDROID
    public VideoPlayer videoPlayer;
#endif

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

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityMessageManager.Instance.SendMessageToFlutter("video:play:" + videoFileNames[index]);
#else
        StartCoroutine(PlayVideoCoroutine(index));
#endif
    }

#if UNITY_EDITOR || !UNITY_ANDROID
    IEnumerator PlayVideoCoroutine(int index)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Video", videoFileNames[index]).Replace("\\", "/");
        videoPlayer.url = path;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.Stop();
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;
        videoPlayer.Play();
    }
#endif
}
