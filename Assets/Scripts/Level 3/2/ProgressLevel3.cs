using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProgressBarL3_2 : MonoBehaviour
{
    public Button cancelButton;
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private const string progressKey = "LevelProgress3";
    private const float targetProgress = 0.66f;

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    private async void OnCancelClicked()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is not authenticated!");
            return;
        }

        Debug.Log($"User ID: {user.UserId}"); // Отладочное сообщение

        var docRef = db.Collection("users").Document(user.UserId);
        try
        {
            var snapshot = await docRef.GetSnapshotAsync();

            float current = 0f;
            if (snapshot.Exists && snapshot.ContainsField(progressKey))
            {
                object val = snapshot.GetValue<object>(progressKey);
                if (val is long l) current = l;
                else if (val is double d) current = (float)d;
                else if (val is float f) current = f;
            }

            Debug.Log($"Current progress: {current}, Target progress: {targetProgress}"); // Отладочное сообщение

            if (targetProgress > current)
            {
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { progressKey, targetProgress }
                };

                await docRef.SetAsync(updates, SetOptions.MergeAll);
                Debug.Log($"Updated {progressKey} to {targetProgress}"); // Отладочное сообщение
            }
            else
            {
                Debug.Log("No need to update progress"); // Отладочное сообщение
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating progress: {e.Message}");
        }

        SceneManager.LoadScene("Home");
    }
}
