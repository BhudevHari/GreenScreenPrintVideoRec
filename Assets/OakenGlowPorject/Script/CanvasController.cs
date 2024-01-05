using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private RectTransform PhotoPageRectTransform;
    [SerializeField] private GameObject PhotoControls;
    [SerializeField] private GameObject AskVideoPageControls;
    [SerializeField] private RectTransform VideoPageRectTransform;
    [SerializeField] private GameObject VideoControls;
    [SerializeField] private GameObject ThankYouPage;
    [SerializeField] private GameObject MainDJTable;

    private void Awake()
    {
        SetPhotoPageUI();
    }

    private void SetPhotoPageUI()
    {
        PhotoControls.SetActive(true);
        VideoControls.SetActive(false);
        Vector2 vector2 = new Vector2(Screen.width, (1800 * Screen.width) / 1200);
        PhotoPageRectTransform.sizeDelta = vector2;
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus && ImageCapturere.Printingphoto)
        {
            //Show If You wanna CaptureVideoPage
            SetAskVideoPageUI();
            ImageCapturere.Printingphoto = false;
        }
    }
    private void SetVideoPageUI()
    {
        PhotoPageRectTransform.gameObject.SetActive(false);
        //VideoPageRectTransform.gameObject.SetActive(true);
        MainDJTable.transform.localPosition += Vector3.down * 1.1f;
        MainDJTable.gameObject.SetActive(true);
        AskVideoPageControls.SetActive(false);
        VideoControls.SetActive(true);
    }
    internal void HideControl()
    {
        AskVideoPageControls.SetActive(false);
        VideoControls.SetActive(false);
    }
    internal void SetThankPageUI()
    {
        MainDJTable.gameObject.SetActive(false);
        AskVideoPageControls.SetActive(false);
        VideoControls.SetActive(false);
        ThankYouPage.SetActive(true);
    }
    private void SetAskVideoPageUI()
    {
        PhotoControls.SetActive(false);
        AskVideoPageControls.SetActive(true);
    }

    public void OnMoveToTakeVideoClick()
    {
        SetVideoPageUI();
    }

    public void OnMoveToRegistrationClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnMoveToThankYouClick()
    {
        SetThankPageUI();
    }

    internal void HidePhotoControls()
    {
        PhotoControls.SetActive(false);
    }
}
