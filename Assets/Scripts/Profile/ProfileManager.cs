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

    public TMP_Text labelSub; // поле для вывода статуса подписки

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
        StartCoroutine(CheckSubscriptionStatus());
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

    private IEnumerator CheckSubscriptionStatus()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("Пользователь не авторизован");
            yield break;
        }

        DocumentReference userDoc = db.Collection("users").Document(user.UserId);
        var task = userDoc.GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("Ошибка при получении данных о подписке: " + task.Exception.Message);
            yield break;
        }

        DocumentSnapshot snapshot = task.Result;
        if (snapshot.Exists && snapshot.ContainsField("subscription"))
        {
            bool hasSubscription = snapshot.GetValue<bool>("subscription");

            if (hasSubscription)
            {
                labelSub.text = "Активна";
                labelSub.color = new Color32(0x17, 0xC9, 0x64, 0xFF); // #17C964
            }
            else
            {
                labelSub.text = "Неактивна";
                labelSub.color = new Color32(0xA1, 0xA1, 0xAA, 0xFF); // #A1A1AA
            }
        }
        else
        {
            // Если поле отсутствует, считаем что подписки нет
            labelSub.text = "Неактивна";
            labelSub.color = new Color32(0xA1, 0xA1, 0xAA, 0xFF);
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
