using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageCapturere : MonoBehaviour
{
    internal static bool Printingphoto = false;
    private static string UserID = null;

    private Texture2D ProcessingTexture, fourbysixImage;
    private RenderTexture m_RenderTexture;
    private Coroutine Photocoroutine, PrintingCoroutine;
    [SerializeField] private GameObject PrintButton;
    [SerializeField] private RawImage OutTextureShow;
    [SerializeField] private TextMeshProUGUI CaptureButtonText;
    [SerializeField] private HorizontalLayoutGroup HLGInstance;
    [SerializeField] private CanvasController CacoInstance;
    [SerializeField] private GameObject MainDJTable;
    RectOffset Spacing, NonSpacing;

    private void Awake()
    {
        Spacing = new RectOffset((int)(Screen.width / 2 * .4f), (int)(Screen.width / 2 * .4f), 0, 0);
        NonSpacing = new RectOffset(0, 0, 0, 0);
        HLGInstance.padding = Spacing;
        OutTextureShow.enabled = false;
    }
    public void CaptureRetakeButtonClick()
    {
        EventSystem.current.SetSelectedGameObject(OutTextureShow.gameObject);
        if (ProcessingTexture == null)
        {
            if (Photocoroutine != null)
            {
                StopCoroutine(Photocoroutine);
            }
            Photocoroutine = StartCoroutine(CaptureImage());
            //File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + fileCounter + ".png", bytes)
        }
        else
        {
            MainDJTable.SetActive(true);
            OutTextureShow.enabled = false;
            PrintButton.SetActive(false);
            CaptureButtonText.text = "Capture";
            HLGInstance.padding = Spacing;
            Destroy(ProcessingTexture);
        }
    }

    private IEnumerator CaptureImage()
    {
        yield return new WaitForSecondsRealtime(.2f);
        Camera.main.Render();

        yield return new WaitForEndOfFrame();

        m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        Camera.main.targetTexture = m_RenderTexture;
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.main.targetTexture;

        Camera.main.Render();

        Texture2D image = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.main.targetTexture.width, Camera.main.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
        Camera.main.targetTexture = null;

        yield return new WaitForEndOfFrame();
        ProcessingTexture = image;

        Camera.main.Render();

        float WidthOfNewImage = Screen.width,
          HeightOfNewImage = (1800 * WidthOfNewImage) / 1200;
        float BaseHeight = Screen.height - HeightOfNewImage;

        //CamerFeed.anchoredPosition += Vector2.up * (BaseHeight * .8f) / 2;
        //BrandingPage.position += Vector3.down * ((BaseHeight / 2) * .35f);
        //TrophyGame.anchoredPosition += Vector2.up * BaseHeight / 2;
        yield return new WaitForSecondsRealtime(.2f);
        Camera.main.Render();
        yield return new WaitForEndOfFrame();

        m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        Camera.main.targetTexture = m_RenderTexture;
        activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.main.targetTexture;

        Camera.main.Render();

        image = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.main.targetTexture.width, Camera.main.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
        Camera.main.targetTexture = null;

        yield return new WaitForEndOfFrame();

        fourbysixImage = new Texture2D((int)WidthOfNewImage, (int)HeightOfNewImage);
        fourbysixImage.SetPixels(image.GetPixels(0, (int)BaseHeight / 2, (int)WidthOfNewImage, (int)HeightOfNewImage));
        fourbysixImage.Apply();

        //TrophyGame.anchoredPosition -= Vector2.up * BaseHeight / 2;
        //CamerFeed.anchoredPosition -= Vector2.up * (BaseHeight * .8f) / 2;
        //BrandingPage.position -= Vector3.down * ((BaseHeight / 2) * .35f);
        OutTextureShow.texture = fourbysixImage;
        OutTextureShow.enabled = true;
        Destroy(m_RenderTexture);

        HLGInstance.padding = NonSpacing;
        PrintButton.SetActive(true);
        MainDJTable.SetActive(false);
        CaptureButtonText.text = "Retake";

    }

    public void OnPrintPhotoButtonClick()
    {
        OutTextureShow.enabled = false;
        if (PrintingCoroutine != null)
        {
            StopCoroutine(PrintingCoroutine);
        }
        PrintingCoroutine = StartCoroutine(PrintPhoto());
    }

    private IEnumerator PrintPhoto()
    {
        yield return new WaitForSecondsRealtime(1);

        string PathToPutPhotoAt = System.IO.Path.Combine(Application.persistentDataPath, "Images");

        if (!Directory.Exists(PathToPutPhotoAt))
        {
            Directory.CreateDirectory(PathToPutPhotoAt);
        }
        Debug.Log(PathToPutPhotoAt);
        string PngFilePath = System.IO.Path.Combine(PathToPutPhotoAt, $"Photo{DateTime.Now.ToString().Replace(':', '_').Replace('/', '_').Replace('\\', '_')}.png");
        Debug.Log(PngFilePath);
        if (File.Exists(PngFilePath))
        {
            File.Delete(PngFilePath);
        }
        FileStream FileData = File.Create(PngFilePath);
        yield return FileData.WriteAsync(fourbysixImage.EncodeToPNG());
        yield return FileData.FlushAsync();
        FileData.Close();
        yield return FileData.DisposeAsync();

#if !UNITY_EDITOR && UNITY_ANDROID
        yield return new WaitForSecondsRealtime(.3f);
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        androidJavaObject.Call("PrintPhoto", PngFilePath);
#elif UNITY_EDITOR
        yield return new WaitForSecondsRealtime(.3f);
#endif
        Printingphoto = true;
        CacoInstance.HidePhotoControls();
    }

    public IEnumerator sendVideoToBucket(string VideoPath)
    {
        if (string.IsNullOrWhiteSpace(VideoPath))
        {
            yield break;
        }
        string apiURL = "";
        WWWForm formData = new WWWForm();
        formData.AddField("userid", UserID);
        string[] Data = VideoPath.Split("/");
        formData.AddBinaryData("video", File.ReadAllBytes(VideoPath), Data[Data.Length - 1]);
        using (UnityWebRequest request = UnityWebRequest.Post(apiURL, formData))
        {
            request.SetRequestHeader("Content-Type", "multipart/form-data;");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("API Failed: " + request.error);
                yield return new WaitForSeconds(5);
                StartCoroutine(sendVideoToBucket(VideoPath));
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API response: " + responseText);

                // Handle API response
            }
        }
    }
}
