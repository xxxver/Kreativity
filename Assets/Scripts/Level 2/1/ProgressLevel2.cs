using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarL2 : MonoBehaviour
{
    public Button cancelButton;
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private const string levelKey = "LevelProgress2";
    private const float targetValue = 0.25f;

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    private async void OnCancelClicked()
    {
        await UpdateProgressIfLessThan(targetValue);
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
            else if (val is float f) current = f;
        }

        if (value - current > 0.001f)
        {
            await docRef.SetAsync(new Dictionary<string, object> { { levelKey, value } }, SetOptions.MergeAll);
            Debug.Log($"Updated {levelKey} to {value}");
        }
    }
}
