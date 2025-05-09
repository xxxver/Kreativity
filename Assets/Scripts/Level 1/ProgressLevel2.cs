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

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }

    private async void OnCancelClicked()
    {
        PlayerPrefs.SetFloat("LevelProgress", 1.0f);
        PlayerPrefs.Save();

        // Award points for level completion
        await AwardLevelCompletion();

        SceneManager.LoadScene("Home");
    }

    private async Task AwardLevelCompletion()
    {
        if (UserData.Instance != null)
        {
            long newBalls = UserData.Instance.balls + 100;
            UserData.Instance.balls = newBalls;

            // Get the current user
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                string userId = user.UserId;

                // Reference to the user's document
                DocumentReference docRef = db.Collection("users").Document(userId);

                // Check if the document exists
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // Document exists, update it
                    await docRef.UpdateAsync(new Dictionary<string, object>
                    {
                        { "Balls", newBalls }
                    });
                }
                else
                {
                    Debug.LogError("User document not found!");
                }
            }
            else
            {
                Debug.LogError("User is not authenticated!");
            }
        }
    }
}
