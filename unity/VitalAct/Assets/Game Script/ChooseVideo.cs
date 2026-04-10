using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

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
            videoButtons[i].onClick.AddListener(() => StartCoroutine(PlayVideo(index)));
        }
    }

    IEnumerator PlayVideo(int index)
    {
        if (index < 0 || index >= videoFileNames.Length) yield break;

        string fileName = videoFileNames[index];
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "Video", fileName).Replace("\\", "/");

#if UNITY_ANDROID && !UNITY_EDITOR
        string destPath = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(destPath))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(streamingPath))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load video: " + www.error);
                    yield break;
                }

                File.WriteAllBytes(destPath, www.downloadHandler.data);
            }
        }

        videoPlayer.url = "file://" + destPath;
#else
        videoPlayer.url = streamingPath;
        yield return null;
#endif

        videoPlayer.source = VideoSource.Url;
        videoPlayer.Stop();
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();
    }
}