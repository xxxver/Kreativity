using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarManager3 : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;
    public GameObject theoryButtonObject;
    public GameObject theoryPanel;
    public Image theoryButtonImage;
    public Sprite theoryBlockedSprite;
    public Sprite theoryActiveSprite;

    private Button theoryButton;
    private FirebaseFirestore db;
    private FirebaseAuth auth;

    private const string levelKey = "LevelProgress3";
    private const string PlayerPrefsKey = "LevelProgress3_Local";

    void Start()
    {
        if (theoryButtonObject != null)
            theoryButton = theoryButtonObject.GetComponent<Button>();

        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // 1. Сначала отображаем локальный кеш, если есть
        float localProgress = PlayerPrefs.GetFloat(PlayerPrefsKey, 0f);
        ApplyProgress(localProgress);

        // 2. Асинхронно грузим прогресс из Firestore
        LoadProgress();
    }

    private async void LoadProgress()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("User is not authenticated.");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            float progress = 0f;

            if (snapshot.Exists && snapshot.ContainsField(levelKey))
            {
                object raw = snapshot.GetValue<object>(levelKey);
                if (raw is long l) progress = l;
                else if (raw is double d) progress = (float)d;
                else if (raw is float f) progress = f;
            }
            else
            {
                await docRef.SetAsync(new Dictionary<string, object> { { levelKey, progress } }, SetOptions.MergeAll);
            }

            // Если пришло обновлённое значение — обновляем PlayerPrefs и UI
            float cachedProgress = PlayerPrefs.GetFloat(PlayerPrefsKey, 0f);
            if (!Mathf.Approximately(progress, cachedProgress))
            {
                PlayerPrefs.SetFloat(PlayerPrefsKey, progress);
                PlayerPrefs.Save();
                ApplyProgress(progress);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading progress: {e.Message}");
        }
    }

    private void ApplyProgress(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;
        UpdateProgressText(progress);
        CheckUnlockTheory(progress);
    }

    private void UpdateProgressText(float progress)
    {
        if (progressText != null)
        {
            int percent = Mathf.RoundToInt(progress * 100);
            progressText.text = $"{percent}%";
        }
    }

    private void CheckUnlockTheory(float progress)
    {
        if (theoryButton == null || theoryButtonImage == null || theoryPanel == null)
        {
            Debug.LogError("UI references are not assigned!");
            return;
        }

        bool unlocked = progress >= 1f;

        theoryButton.interactable = unlocked;
        theoryButtonImage.sprite = unlocked ? theoryActiveSprite : theoryBlockedSprite;
        ShowTheoryPanel(unlocked);
    }

    private void ShowTheoryPanel(bool show)
    {
        if (theoryPanel == null) return;
        foreach (Transform child in theoryPanel.transform)
        {
            child.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// Очистить локальный кеш прогресса (например, при смене аккаунта)
    /// </summary>
    public static void ClearCachedProgress()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
    }
}
