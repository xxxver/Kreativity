using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarL3_3 : MonoBehaviour
{
    public Button cancelButton;
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private const string progressKey = "LevelProgress3";
    private const float targetProgress = 1.0f;
    private const string completeKey = "Level3Completed";

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }

    private async void OnCancelClicked()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is not authenticated!");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            float currentProgress = 0f;
            if (snapshot.Exists && snapshot.ContainsField(progressKey))
            {
                object val = snapshot.GetValue<object>(progressKey);
                if (val is long l) currentProgress = l;
                else if (val is double d) currentProgress = (float)d;
                else if (val is float f) currentProgress = f;
            }

            bool alreadyCompleted = snapshot.Exists &&
                snapshot.ContainsField(completeKey) &&
                snapshot.GetValue<bool>(completeKey);

            var updates = new Dictionary<string, object>();

            if (currentProgress < targetProgress)
            {
                updates[progressKey] = targetProgress;
                Debug.Log($"Progress updated to {targetProgress}");
            }

            if (!alreadyCompleted)
            {
                long newBalls = UserData.Instance != null ? UserData.Instance.balls + 100 : 100;
                if (UserData.Instance != null)
                    UserData.Instance.balls = newBalls;

                updates["Balls"] = newBalls;
                updates[completeKey] = true;

                Debug.Log("Reward granted and level marked as completed.");
            }

            if (updates.Count > 0)
            {
                await docRef.SetAsync(updates, SetOptions.MergeAll);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating progress: {e.Message}");
        }

        SceneManager.LoadScene("Home");
    }
}
