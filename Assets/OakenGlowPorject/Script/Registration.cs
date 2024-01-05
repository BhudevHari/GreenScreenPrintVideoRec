using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text;

public class Registration : MonoBehaviour
{
    internal static string UserID = null;
    [SerializeField] private bool UseAPI = true;

    [Header("Element")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField mobileInput;
    [SerializeField] private TMP_InputField ageInput;
    //[SerializeField] private TMP_Dropdown cityInput;
    [SerializeField] private TMP_Dropdown StateInput;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Toggle CheckBox;
    [SerializeField] private Button SubmitButton;

    [Header("Form")]
    [SerializeField] private GameObject Form;
    [SerializeField] private GameObject FormParent;
    [SerializeField] private GameObject OutletParent;

    private const string apiURL = "https://smokyparty.com/AR/register.php";

    public void SubmitForm()
    {
        Form.SetActive(false);
        string name = nameInput.text;
        string mobile = mobileInput.text;
        string age = ageInput.text;
        //string city = cityInput.options[cityInput.value].text;
        string State = StateInput.options[StateInput.value].text;


        // Input validation
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile) ||
            string.IsNullOrEmpty(age) /*|| string.IsNullOrEmpty(city)*/)
        {
            Form.SetActive(true);
            errorText.text = "Please fill in all fields.";
            return;
        }

        // Mobile number validation
        if (!IsValidMobileNumber(mobile))
        {
            Form.SetActive(true);
            errorText.text = "Invalid mobile number.";
            return;
        }

        // Age validation
        if (!int.TryParse(age, out int ageValue))
        {
            errorText.text = "Invalid age format.";
            Form.SetActive(true);
            return;
        }

        if (ageValue < 21)
        {
            errorText.text = "Age must be over 21 years.";
            Form.SetActive(true);
            return;
        }

        //if (city == "Select City")
        //{
        //    errorText.text = "Please Select Your city";
        //    Form.SetActive(true);
        //    return;
        //}
        if (State == "Select State")
        {
            errorText.text = "Please Select Your State";
            Form.SetActive(true);
            return;
        }

        //if (OutletParent.activeSelf)
        //{
        //    if (OuletCode == "Select Outlet Code")
        //    {
        //        errorText.text = "Please Select Your Outlet";
        //        Form.SetActive(true);
        //        return;
        //    }
        //}
        if (!CheckBox.isOn)
        {
            errorText.text = "Please Accept Terms And Condition";
            Form.SetActive(true);
            return;
        }

        errorText.text = "";
        nameInput.text = "";
        mobileInput.text = "";
        ageInput.text = "";
        StateInput.SetValueWithoutNotify(0);
        //cityInput.SetValueWithoutNotify(0);

        if (UseAPI)
        {
            // Send API request
            StartCoroutine(SendRequest(JsonUtility.ToJson(new SendingData(name, mobile, age, /*city,*/ State))));
        }
        else
        {
            SceneManager.LoadScene(1);
            //UIMInstance.SetFrame(OBJdetectinstance.MainTexture);
            FormParent.SetActive(false);
            Debug.Log("API request successful!");
        }
    }

    private bool IsValidMobileNumber(string number)
    {
        return !string.IsNullOrEmpty(number) && number.Length == 10;
    }

    private IEnumerator SendRequest(string formData)
    {
        SubmitButton.interactable = false;
        using (UnityWebRequest request = UnityWebRequest.Post(apiURL, ""))
        {
            UploadHandlerRaw JsonDataBytes = new UploadHandlerRaw(Encoding.UTF8.GetBytes(formData));
            JsonDataBytes.contentType = "application/json";
            request.uploadHandler = JsonDataBytes;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("API Failed: " + request.error);
                errorText.text = request.error;
                Form.SetActive(true);
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API response: " + responseText);

                // Handle API response
                HandleResponse(responseText);
            }
        }
        SubmitButton.interactable = true;
    }

    private void HandleResponse(string responseText)
    {
        // Parse the JSON response
        ResponseData response = JsonUtility.FromJson<ResponseData>(responseText);

        // Check if the status is false
        if (!response.status)
        {
            Debug.LogError("API request failed: " + response.message);

            // Display the message in Unity (for example, on a UI Text element)
            errorText.text = "No Internet Connection";
            Form.SetActive(true);
        }
        else
        {

            FormParent.SetActive(false);
            Debug.Log("API request successful!");
            //if (!webCamTextureToMatHelper.IsPlaying())
            //{
            //    webCamTextureToMatHelper.Play();
            //}
            //UIMInstance.SetFrame(OBJdetectinstance.MainTexture);
            SceneManager.LoadScene(1);
            UserID = response.userid.ToString();
        }
    }

    public void StateSelect(int Value)
    {
        //OutletParent.SetActive(Value == 14);
    }

    [System.Serializable]
    private class ResponseData
    {
        public string message;
        public bool status;
        public long userid;
    }
    [System.Serializable]
    private class SendingData
    {
        public string name;
        public string mobile;
        public string age;
        public string state;

        internal SendingData(string name, string mobile, string age, /*string city,*/ string state)
        {
            this.name = name;
            this.mobile = mobile;
            this.age = age;
            //this.city = city;
            this.state = state;
        }
    }
}
