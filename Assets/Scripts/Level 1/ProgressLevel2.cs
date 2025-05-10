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
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    public string levelKey = "Level1Completed";
    public float levelProgressValue = 1.0f; // например 100% прогресса

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }

    private async void OnCancelClicked()
    {
        float savedProgress = PlayerPrefs.GetFloat("LevelProgress", 0f);

        if (levelProgressValue > savedProgress)
        {
            PlayerPrefs.SetFloat("LevelProgress", levelProgressValue);
            PlayerPrefs.Save();
        }

        await CheckAndAwardIfNotCompleted();
    }

    private async Task CheckAndAwardIfNotCompleted()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is not authenticated!");
            return;
        }

        string userId = user.UserId;
        DocumentReference docRef = db.Collection("users").Document(userId);

        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            Dictionary<string, object> userData = snapshot.Exists ? snapshot.ToDictionary() : new Dictionary<string, object>();

            // Проверяем, был ли уровень уже пройден
            bool levelCompleted = userData.ContainsKey(levelKey) && (bool)userData[levelKey];
            if (levelCompleted)
            {
                Debug.Log("Level already completed, skipping reward.");
            }
            else
            {
                await AwardLevelCompletion(docRef, userData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking or awarding level completion: {e.Message}");
        }

        SceneManager.LoadScene("Home");
    }

    private async Task AwardLevelCompletion(DocumentReference docRef, Dictionary<string, object> userData)
    {
        if (UserData.Instance == null)
        {
            Debug.LogError("UserData.Instance is null!");
            return;
        }

        long newBalls = UserData.Instance.balls + 100;
        UserData.Instance.balls = newBalls;

        // Обновляем данные в Firestore
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "Balls", newBalls },
            { levelKey, true }
        };

        try
        {
            await docRef.SetAsync(updates, SetOptions.MergeAll);
            Debug.Log("Awarded points and marked level as completed.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating Firestore document: {e.Message}");
        }
    }
}
