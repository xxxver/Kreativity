using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarL22 : MonoBehaviour
{
    public Button cancelButton;
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private const string levelKey = "LevelProgress2";
    private const float targetValue = 0.5f;
    private const float threshold = 0.75f; // Изменено на 0.75

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
            Debug.LogWarning("❌ Пользователь не авторизован");
            SceneManager.LoadScene("Home");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            float current = 0f;
            if (snapshot.Exists && snapshot.ContainsField(levelKey))
            {
                object val = snapshot.GetValue<object>(levelKey);
                if (val is long l) current = l;
                else if (val is double d) current = (float)d;
                else if (val is float f) current = f;
            }

            Debug.Log($"🟡 Текущий прогресс: {current}, Порог: {threshold}");

            if (current < threshold)
            {
                await docRef.SetAsync(new Dictionary<string, object> { { levelKey, targetValue } }, SetOptions.MergeAll);
                Debug.Log($"✅ Обновлён {levelKey} до {targetValue} (было меньше {threshold})");
            }
            else
            {
                Debug.Log($"ℹ Прогресс {levelKey} уже >= {threshold} — не обновляем");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при обновлении прогресса: {e.Message}");
        }

        SceneManager.LoadScene("Home");
    }
}
