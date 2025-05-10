using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    public Button saveButton;
    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;

    public Button extraOpenButton1;
    public Button extraOpenButton2;

    private string originalName;
    private string originalEmail;

    private void Start()
    {
        saveButton.interactable = false;

        saveButton.onClick.AddListener(SaveData);

        extraOpenButton1.onClick.AddListener(() => LoadScene("Home"));
        extraOpenButton2.onClick.AddListener(() => LoadScene("Home"));

        FetchDataFromUserData();
    }

    private void FetchDataFromUserData()
    {
        originalName = UserData.Instance.userName;
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

    private void SaveData()
    {
        UserData.Instance.SetUserData(nameInputField.text, emailInputField.text, UserData.Instance.balls);

        Debug.Log("Data saved successfully!");

        saveButton.interactable = false;

        originalName = nameInputField.text;
        originalEmail = emailInputField.text;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
