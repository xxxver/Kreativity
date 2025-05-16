using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProgressBar : MonoBehaviour
{
    public Button cancelButton;
    public float targetProgressValue = 0.5f;
    private string levelKey = "LevelProgress1";

    private FirebaseAuth auth;
    private FirebaseFirestore db;

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
            Debug.LogError("User not authenticated.");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        float savedProgress = 0f;

        if (snapshot.Exists && snapshot.ContainsField(levelKey))
        {
            object rawValue = snapshot.GetValue<object>(levelKey);
            if (rawValue is long l) savedProgress = l;
            else if (rawValue is double d) savedProgress = (float)d;
            else if (rawValue is float f) savedProgress = f;
        }

        if (targetProgressValue > savedProgress)
        {
            var updates = new Dictionary<string, object>
            {
                { levelKey, targetProgressValue }
            };

            await docRef.SetAsync(updates, SetOptions.MergeAll);
            Debug.Log($"LevelProgress1 updated to {targetProgressValue}");
        }

        SceneManager.LoadScene("Home");
    }
}
