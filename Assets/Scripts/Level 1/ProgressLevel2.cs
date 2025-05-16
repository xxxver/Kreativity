using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBar2 : MonoBehaviour
{
    public Button cancelButton;
    private string levelKey = "LevelProgress1";
    private string completeKey = "Level1Completed";

    private FirebaseFirestore db;
    private FirebaseAuth auth;

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

        float savedProgress = 0f;
        if (snapshot.Exists && snapshot.ContainsField(levelKey))
        {
            object val = snapshot.GetValue<object>(levelKey);
            if (val is long l) savedProgress = l;
            else if (val is double d) savedProgress = (float)d;
            else if (val is float f) savedProgress = f;
        }

        bool alreadyCompleted = snapshot.Exists &&
            snapshot.ContainsField(completeKey) &&
            snapshot.GetValue<bool>(completeKey);

        Dictionary<string, object> updates = new Dictionary<string, object>();

        if (!alreadyCompleted)
        {
            if (UserData.Instance == null)
            {
                Debug.LogError("UserData.Instance is null!");
                return;
            }

            long newBalls = UserData.Instance.balls + 100;
            UserData.Instance.balls = newBalls;

            updates["Balls"] = newBalls;
            updates[completeKey] = true;

            Debug.Log("Reward granted and level marked as completed.");
        }

        if (savedProgress < 1f)
        {
            updates[levelKey] = 1f;
            Debug.Log("LevelProgress1 updated to 1f.");
        }

        if (updates.Count > 0)
        {
            await docRef.SetAsync(updates, SetOptions.MergeAll);
        }

        SceneManager.LoadScene("Home");
    }
}
