using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{
    public Button confirmButton;
    public GameObject winPanel;
    public Button goHomeButton;
    public Button nextLevelButton;

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    public string levelKey = "Level3Completed";
    public float levelProgressValue = 1.0f; // например 100% прогресса

    private void Start()
    {
        Debug.Log("Start method called.");

        winPanel.SetActive(false);

        // Initialize Firebase
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {sceneName}");

        if (sceneName == "Level3-1")
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
            nextLevelButton.onClick.AddListener(() => SceneManager.LoadScene("Level3-2"));
            Debug.Log("Level3-1 setup complete.");
        }
        else if (sceneName == "Level3-2")
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
            nextLevelButton.onClick.AddListener(() => SceneManager.LoadScene("Level3-3"));
            Debug.Log("Level3-2 setup complete.");
        }
        else if (sceneName == "Level3-3")
        {
            confirmButton.onClick.AddListener(ConfirmSelection);
            nextLevelButton.onClick.AddListener(() => SceneManager.LoadScene("Home"));
            Debug.Log("Level3-3 setup complete.");
        }

        goHomeButton.onClick.AddListener(() => SceneManager.LoadScene("Home"));
        Debug.Log("Go Home button setup complete.");
    }

    private async void ConfirmSelection()
    {
        Debug.Log("ConfirmSelection method called.");

        SelectableElement selected = SelectableElement.GetSelected();
        if (selected != null)
        {
            Debug.Log("Selected element found.");

            if (selected.isCorrectAnswer)
            {
                selected.ShowCorrect();
                StartCoroutine(ShowPanelAfterDelay());
                Debug.Log("Correct answer selected.");

                // Save progress and update database
                await SaveProgressAndUpdateDatabase();
            }
            else
            {
                selected.ShowIncorrect();
                Debug.Log("Incorrect answer selected.");
            }
        }
        else
        {
            Debug.LogError("No selected element found.");
        }
    }

    private IEnumerator ShowPanelAfterDelay()
    {
        Debug.Log("ShowPanelAfterDelay coroutine started.");

        yield return new WaitForSeconds(1f);
        winPanel.SetActive(true);
        Debug.Log("Win panel activated.");
    }

    private async Task SaveProgressAndUpdateDatabase()
    {
        Debug.Log("SaveProgressAndUpdateDatabase method called.");

        float savedProgress = PlayerPrefs.GetFloat("LevelProgress3", 0f);
        Debug.Log($"Saved progress: {savedProgress}");

        if (levelProgressValue > savedProgress)
        {
            PlayerPrefs.SetFloat("LevelProgress3", levelProgressValue);
            PlayerPrefs.Save();
            Debug.Log("Level progress saved to PlayerPrefs.");
        }

        await CheckAndAwardIfNotCompleted();
    }

    private async Task CheckAndAwardIfNotCompleted()
    {
        Debug.Log("CheckAndAwardIfNotCompleted method called.");

        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User is not authenticated!");
            return;
        }

        Debug.Log("User is authenticated. User ID: " + user.UserId);

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
    }

    private async Task AwardLevelCompletion(DocumentReference docRef, Dictionary<string, object> userData)
    {
        Debug.Log("AwardLevelCompletion method called.");

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
