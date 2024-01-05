using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage m_Image;

    internal void LoadVideo(string _url)
    {
        gameObject.SetActive(true);
        videoPlayer.enabled = true;
        videoPlayer.url = _url;
        videoPlayer.isLooping = true;
        videoPlayer.prepareCompleted += VideoIsPrepared;
        videoPlayer.Play();
    }
    RenderTexture renderTexture;
    private void VideoIsPrepared(VideoPlayer source)
    {
        Debug.Log(source.height + " " + source.width);
        renderTexture = new RenderTexture((int)source.width / 2, (int)source.height / 2, 16, RenderTextureFormat.ARGB32);
        renderTexture.memorylessMode = RenderTextureMemoryless.Color;
        source.targetTexture = renderTexture;
        m_Image.texture = renderTexture;
    }
    public void ClearVideo()
    {
        gameObject.SetActive(false);
        renderTexture.DiscardContents();
        Destroy(renderTexture);
    }
}
