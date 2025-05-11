using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarl24 : MonoBehaviour
{
public Button cancelButton;
private FirebaseFirestore db;
private FirebaseAuth auth;
public string levelKey = "Level2Completed"; // имя поля в Firestore, соответствующее уровню
public float levelProgressValue = 1.0f; // 100% прогресс

void Start()
{
    cancelButton.onClick.AddListener(OnCancelClicked);
    db = FirebaseFirestore.DefaultInstance;
    auth = FirebaseAuth.DefaultInstance;
}

private async void OnCancelClicked()
{
    float savedProgress = PlayerPrefs.GetFloat("LevelProgress2", 0f);

    if (levelProgressValue > savedProgress)
    {
        PlayerPrefs.SetFloat("LevelProgress2", levelProgressValue);
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
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

    Dictionary<string, object> updates = new Dictionary<string, object>();

    if (!snapshot.Exists)
    {
        Debug.Log("User document not found, creating a new one.");
        updates["Balls"] = 100; // Начальное значение
        updates[levelKey] = true;
    }
    else
    {
        Dictionary<string, object> userData = snapshot.ToDictionary();
        bool levelCompleted = userData.ContainsKey(levelKey) && (bool)userData[levelKey];

        if (!levelCompleted)
        {
            long newBalls = UserData.Instance.balls + 100;
            UserData.Instance.balls = newBalls;
            updates["Balls"] = newBalls;
            updates[levelKey] = true;
        }
        else
        {
            Debug.Log("Level already completed, skipping reward.");
        }
    }

    if (updates.Count > 0)
    {
        await docRef.SetAsync(updates, SetOptions.MergeAll);
        Debug.Log("Awarded points and marked level as completed.");
    }

    SceneManager.LoadScene("Home");
}

private async Task AwardLevelCompletion(DocumentReference docRef)
{
    if (UserData.Instance == null)
    {
        Debug.LogError("UserData.Instance is null!");
        return;
    }

    long newBalls = UserData.Instance.balls + 100;
    UserData.Instance.balls = newBalls;

    Dictionary<string, object> updates = new Dictionary<string, object>
    {
        { "Balls", newBalls },
        { levelKey, true }
    };

    await docRef.UpdateAsync(updates);

    Debug.Log("Awarded points and marked level as completed.");
}
}