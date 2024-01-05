using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSwapper : MonoBehaviour
{
    [SerializeField] private VideoClip[] ShowingVideoClip;
    [SerializeField] private VideoPlayer m_VideoPlayer;
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private RawImage m_RawImage;
    int CurrentIndex = -1;

    RenderTexture OuputTexture;


    private void Awake()
    {
        Swap();
        m_VideoPlayer.prepareCompleted += M_VideoPlayer_prepareCompleted;
    }

    private void M_VideoPlayer_prepareCompleted(VideoPlayer source)
    {
        if (OuputTexture != null)
        {
            Destroy(OuputTexture);
        }
        Vector2 FinalSize = new Vector2(source.width, source.height);
        float DifferenceFactor = Screen.height / FinalSize.y;
        m_RectTransform.sizeDelta = FinalSize * DifferenceFactor;
        OuputTexture = new RenderTexture((int)source.width * 2, (int)source.height * 2, 0, RenderTextureFormat.ARGB32);
        m_VideoPlayer.targetTexture = OuputTexture;
        m_RawImage.texture = OuputTexture;

    }

    public void Swap()
    {
        m_VideoPlayer.clip = ShowingVideoClip[(++CurrentIndex) % ShowingVideoClip.Length];
    }
}
