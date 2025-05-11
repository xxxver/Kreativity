using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;
using UnityEngine.Networking;

public class ProfileManager : MonoBehaviour
{
    public Button saveButton;
    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField verificationCodeInputField;
    public Button sendVerificationCodeButton;
    public Button verifyCodeButton;

    public Button extraOpenButton1;
    public Button extraOpenButton2;

    private string originalName;
    private string originalEmail;
    private string verificationCode;

    private void Start()
    {
        saveButton.interactable = false;
        verifyCodeButton.interactable = false;

        saveButton.onClick.AddListener(SaveData);
        sendVerificationCodeButton.onClick.AddListener(SendVerificationCode);
        verifyCodeButton.onClick.AddListener(VerifyCode);

        extraOpenButton1.onClick.AddListener(() => LoadScene("Home"));
        extraOpenButton2.onClick.AddListener(() => LoadScene("Home"));

        FetchDataFromUserData();
    }

    private void FetchDataFromUserData()
    {
        originalName = UserData.Instance.Name;
        originalEmail = UserData.Instance.email;

        nameInputField.text = originalName;
        emailInputField.text = originalEmail;

        nameInputField.onValueChanged.AddListener(delegate { CheckForChanges(); });
        emailInputField.onValueChanged.AddListener(delegate { CheckForChanges(); });
    }

    private void CheckForChanges()
    {
        bool isChanged = (nameInputField.text != originalName) || (emailInputField.text != originalEmail);
        saveButton.interactable = isChanged;
    }

    private void SendVerificationCode()
    {
        verificationCode = GenerateRandomCode();
        string email = emailInputField.text;

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("Email is empty!");
            return;
        }

        Debug.Log("Generated verification code: " + verificationCode);
        StartCoroutine(SendVerificationCodeRequest(email, verificationCode));
    }

    private string GenerateRandomCode()
    {
        System.Random random = new System.Random();
        string code = random.Next(100000, 999999).ToString();
        Debug.Log("Generated code: " + code);
        return code;
    }

    private IEnumerator SendVerificationCodeRequest(string email, string code)
    {
        string url = "http://localhost:3000/send-verification-code";
        Debug.Log("Sending request to: " + url);
        Debug.Log("Email: " + email);
        Debug.Log("Code: " + code);

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("code", code);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending verification code: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Verification code sent: " + www.downloadHandler.text);
            }
        }
    }

    private void VerifyCode()
    {
        if (string.IsNullOrEmpty(verificationCodeInputField.text))
        {
            Debug.LogError("Verification code input is empty!");
            return;
        }

        if (string.IsNullOrEmpty(verificationCode))
        {
            Debug.LogError("Verification code is empty!");
            return;
        }

        Debug.Log("Verification code input: " + verificationCodeInputField.text);
        Debug.Log("Verification code: " + verificationCode);

        if (verificationCodeInputField.text == verificationCode)
        {
            SaveData();
        }
        else
        {
            Debug.LogError("Invalid verification code!");
        }
    }

    private void SaveData()
    {
        string newName = nameInputField.text;
        string newEmail = emailInputField.text;

        UserData.Instance.SetUserData(newName, newEmail, UserData.Instance.balls);

        Debug.Log("Data saved successfully!");

        saveButton.interactable = false;
        verifyCodeButton.interactable = false;

        originalName = newName;
        originalEmail = newEmail;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
