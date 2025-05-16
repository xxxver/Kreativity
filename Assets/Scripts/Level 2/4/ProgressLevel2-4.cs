using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarL24 : MonoBehaviour
{
    public Button cancelButton;
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private string levelProgressKey = "LevelProgress2";
    private string completionKey = "Level2Completed";

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
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        bool alreadyCompleted = snapshot.Exists &&
            snapshot.ContainsField(completionKey) &&
            snapshot.GetValue<bool>(completionKey);

        float savedProgress = 0f;
        if (snapshot.Exists && snapshot.ContainsField(levelProgressKey))
        {
            object val = snapshot.GetValue<object>(levelProgressKey);
            if (val is long l) savedProgress = l;
            else if (val is double d) savedProgress = (float)d;
            else if (val is float f) savedProgress = f;
        }

        var updates = new Dictionary<string, object>();

        // Обновим прогресс, если меньше 1
        if (savedProgress < 1f)
        {
            updates[levelProgressKey] = 1f;
        }

        if (!alreadyCompleted)
        {
            long newBalls = UserData.Instance != null ? UserData.Instance.balls + 100 : 100;
            if (UserData.Instance != null) UserData.Instance.balls = newBalls;

            updates["Balls"] = newBalls;
            updates[completionKey] = true;

            Debug.Log("🎯 Level 2 completed: +100 balls, flag set, progress = 1.");
        }
        else
        {
            Debug.Log("ℹ Level 2 was already completed — progress updated to 1.");
        }

        if (updates.Count > 0)
        {
            await docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        SceneManager.LoadScene("Home");
    }
}
