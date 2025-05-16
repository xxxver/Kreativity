using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarL23 : MonoBehaviour
{
    public Button cancelButton;
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private string levelKey = "LevelProgress2";

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    private async void OnCancelClicked()
    {
        await UpdateProgressIfLessThan(0.75f);
        SceneManager.LoadScene("Home");
    }

    private async Task UpdateProgressIfLessThan(float value)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        var docRef = db.Collection("users").Document(user.UserId);
        var snapshot = await docRef.GetSnapshotAsync();

        float current = 0f;
        if (snapshot.Exists && snapshot.ContainsField(levelKey))
        {
            object val = snapshot.GetValue<object>(levelKey);
            if (val is long l) current = l;
            else if (val is double d) current = (float)d;
        }

        if (value > current)
        {
            await docRef.SetAsync(new Dictionary<string, object> { { levelKey, value } }, SetOptions.MergeAll);
        }
    }
}
