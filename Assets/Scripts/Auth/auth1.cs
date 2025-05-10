using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserData : MonoBehaviour
{
    public static UserData Instance;

    public string Name;
    public string email;
    public long balls;

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // сохраняем при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Exception != null)
            {
                Debug.LogError("Failed to initialize Firebase: " + task.Exception);
                return;
            }

            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            user = auth.CurrentUser;
        });
    }

    public void SetUserData(string name, string email, long balls)
    {
        this.Name = name;
        this.email = email;
        this.balls = balls;

        // Save data to Firestore
        SaveDataToFirestore();
    }

    private void SaveDataToFirestore()
    {
        if (db == null || user == null)
        {
            Debug.LogError("Firestore or User is not initialized.");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "Name", Name },
            { "email", email },
            { "balls", balls }
        };

        docRef.SetAsync(userData).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Data saved to Firestore successfully!");
            }
            else
            {
                Debug.LogError("Failed to save data to Firestore: " + task.Exception);
            }
        });
    }
}
