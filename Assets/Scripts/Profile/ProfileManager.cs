using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

public class ProfileManager : MonoBehaviour
{
    public Button saveButton;
    public TMP_InputField nameInputField;

    public Button extraOpenButton1;
    public Button extraOpenButton2;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private string originalName;
    private string pendingName;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        saveButton.interactable = false;
        saveButton.onClick.AddListener(OnSaveButtonClicked);

        extraOpenButton1.onClick.AddListener(() => LoadScene("Home"));
        extraOpenButton2.onClick.AddListener(() => LoadScene("Home"));

        FetchDataFromUserData();
    }

    private void FetchDataFromUserData()
    {
        originalName = UserData.Instance.Name;
        nameInputField.text = originalName;

        nameInputField.onValueChanged.AddListener(delegate { CheckForChanges(); });
    }

    private void CheckForChanges()
    {
        bool isChanged = nameInputField.text.Trim() != originalName;
        saveButton.interactable = isChanged;
    }

    private void OnSaveButtonClicked()
    {
        pendingName = nameInputField.text.Trim();
        StartCoroutine(SaveName());
    }

    private IEnumerator SaveName()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("Пользователь не авторизован");
            yield break;
        }

        DocumentReference userDoc = db.Collection("users").Document(user.UserId);
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "Name", pendingName }
        };

        var firestoreTask = userDoc.UpdateAsync(updates);
        yield return new WaitUntil(() => firestoreTask.IsCompleted);

        if (firestoreTask.Exception != null)
        {
            Debug.LogError("Ошибка при обновлении имени в Firestore: " + firestoreTask.Exception.Message);
            yield break;
        }

        UserData.Instance.SetUserData(pendingName, UserData.Instance.email, UserData.Instance.balls);
        originalName = pendingName;

        saveButton.interactable = false;
        Debug.Log("✅ Имя успешно обновлено");
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
