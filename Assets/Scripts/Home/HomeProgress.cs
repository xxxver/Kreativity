using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarManager : MonoBehaviour
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
    private const string levelKey = "LevelProgress1";
    private const string PlayerPrefsKey = "LevelProgress1_Local";

    private void Awake()
    {
        if (theoryButtonObject != null)
            theoryButton = theoryButtonObject.GetComponent<Button>();
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // 1. Мгновенно показать локальные данные (если есть)
        float localProgress = PlayerPrefs.GetFloat(PlayerPrefsKey, 0f);
        ApplyProgressToUI(localProgress);

        // 2. Асинхронно обновить из Firestore (перезапишет если есть новое значение)
        LoadProgressFromFirestore();
    }

    private async void LoadProgressFromFirestore()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("User is not authenticated. Skipping Firestore progress load.");
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        float progress = 0f;

        if (snapshot.Exists && snapshot.ContainsField(levelKey))
        {
            object rawValue = snapshot.GetValue<object>(levelKey);
            if (rawValue is long l) progress = l;
            else if (rawValue is double d) progress = (float)d;
            else if (rawValue is float f) progress = f;
        }

        Debug.Log($"[LevelProgress1] Firestore loaded value: {progress}");

        // 3. Если пришло новое значение — обновить PlayerPrefs и UI
        float localProgress = PlayerPrefs.GetFloat(PlayerPrefsKey, 0f);
        if (!Mathf.Approximately(progress, localProgress))
        {
            PlayerPrefs.SetFloat(PlayerPrefsKey, progress);
            PlayerPrefs.Save();
            ApplyProgressToUI(progress);
        }
    }

    private void ApplyProgressToUI(float progress)
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
            Debug.LogWarning("⚠️ UI не привязан, CheckUnlockTheory пропущен.");
            return;
        }

        bool unlocked = Mathf.Approximately(progress, 1.0f);

        theoryButton.interactable = unlocked;
        theoryButtonImage.sprite = unlocked ? theoryActiveSprite : theoryBlockedSprite;
        ShowTheoryPanel(unlocked);
    }

    private void ShowTheoryPanel(bool show)
    {
        foreach (Transform child in theoryPanel.transform)
        {
            child.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// Очистить локальный прогресс (например, при выходе из аккаунта)
    /// </summary>
    public static void ClearCachedProgress()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
    }
}
