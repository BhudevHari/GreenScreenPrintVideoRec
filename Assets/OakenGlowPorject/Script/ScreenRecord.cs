using FFmpegUnityBind2;
using FFmpegUnityBind2.Components;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class ScreenRecord : MonoBehaviour, IFFmpegCallbacksHandler
{
    [SerializeField] private Camera RecCamera;
    [SerializeField] private Camera SecCamera;
    [SerializeField] private RawImage RecCameraOutCamer;
    [SerializeField] private FFmpegREC FFmpegREC;
    [SerializeField] private GameObject InteractionLayer;
    [SerializeField] private GameObject Loader;
    [SerializeField] private Material OutMaterial;
    [SerializeField] private VideoPlayerController VPCInstance;
    [SerializeField] private CanvasController CaCoInstance;
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private AudioSource m_AudioToPlay;

    WebCamTexture DataTexture;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DataTexture = new WebCamTexture(Screen.width, Screen.height);
        DataTexture.Play();
        OutMaterial.SetTexture("_MainTex", DataTexture);
        //PlaneWithOutput.transform.localScale = Vector3.right * (DataTexture.width / 500f) + Vector3.forward * (DataTexture.height / 500f);
        //OnStart();
        //Vector2 FinalSize = new Vector2(source.width, source.height);
        float DifferenceFactor;
        m_RectTransform.localEulerAngles = new Vector3(0, 0, -DataTexture.videoRotationAngle);
        DifferenceFactor = Screen.height / DataTexture.height;
        m_RectTransform.sizeDelta = new Vector2(DataTexture.width, DataTexture.height);
        m_AudioToPlay.Stop();
        //Invoke(nameof(OnStop), 1);
    }
    string SaveFile;
    public void OnStart()
    {
        string DirectoryToSaveFileIn = Path.Combine(Application.persistentDataPath, "RecordVideo");
        Debug.Log(DirectoryToSaveFileIn);
        if (!Directory.Exists(DirectoryToSaveFileIn))
        {
            Directory.CreateDirectory(DirectoryToSaveFileIn);
        }
        m_AudioToPlay.Stop();
        m_AudioToPlay.Play();
        m_AudioToPlay.time = 1f;
        DateTime date = new DateTime(1970, 1, 1, 1, 0, 0, 0);
        SaveFile = Path.Combine(DirectoryToSaveFileIn, "Video" + (DateTime.Now - date).TotalMilliseconds + ".mp4");
        FFmpegREC.StartREC(SaveFile, this);
    }
    public void OnStop()
    {
        FFmpegREC.StopREC();
        m_AudioToPlay.Stop();
    }

    bool IsRecording = false, IsProcessing = false;

    public void StartRecording()
    {
        if (!IsRecording && !IsProcessing)
        {
            InteractionLayer.SetActive(false);
            SecCamera.gameObject.SetActive(true);
            OnStart();
            RecCameraOutCamer.enabled = true;
            RecCameraOutCamer.texture = RecCamera.targetTexture;
            IsRecording = true;
            Invoke(nameof(StartRecording), 7);
        }
        else if (IsRecording)
        {
            Loader.SetActive(true);
            OnStop();
            IsRecording = false;
            IsProcessing = true;
        }

    }

    public void OnStart(long executionId)
    {

    }

    public void OnLog(long executionId, string message)
    {
        Debug.Log($"On Log. Execution Id: {executionId}. Message: {message}");
        message.Contains("output");
    }

    public void OnWarning(long executionId, string message)
    {

    }

    public void OnError(long executionId, string message)
    {

    }

    public void OnSuccess(long executionId)
    {
        IsProcessing = false;
        InteractionLayer.SetActive(true);
        SecCamera.gameObject.SetActive(false);
        RecCamera.targetTexture = null;
        RecCameraOutCamer.texture = null;
        RecCameraOutCamer.enabled = false;
        CaCoInstance.HideControl();
        Invoke(nameof(ShowVideo), 5);
    }
    public void ShowVideo()
    {
        //VPCInstance.LoadVideo("file://" + SaveFile);
        StartCoroutine(UploadFile(SaveFile));
    }
    public IEnumerator UploadFile(string FileCompletePath)
    {
        if (Registration.UserID == null)
        {
            CaCoInstance.SetThankPageUI();
            Loader.SetActive(false);
            yield break;
        }
        WWWForm form = new WWWForm();
        form.AddBinaryData("video", File.ReadAllBytes(FileCompletePath), FileCompletePath.Trim().Split('\\').Last());
        form.AddField("userid", Registration.UserID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://smokyparty.com/AR/uploadVideoToS3.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                StartCoroutine(UploadFile(FileCompletePath));
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                CaCoInstance.SetThankPageUI();
                Loader.SetActive(false);

            }
        }
    }

    public void OnCanceled(long executionId)
    {

    }

    public void OnFail(long executionId)
    {

    }
}
