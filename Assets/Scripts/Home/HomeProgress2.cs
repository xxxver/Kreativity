using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProgressBarManager2 : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;
    public GameObject theoryButtonObject;
    public GameObject theoryPanel;
    public Image theoryButtonImage;
    public Sprite theoryBlockedSprite;
    public Sprite theoryActiveSprite;
    public GameObject progressUIRoot;

    private Button theoryButton;
    private FirebaseFirestore db;
    private FirebaseAuth auth;

    private const string levelKey = "LevelProgress2";
    private const string PlayerPrefsKey = "LevelProgress2_Local";

    private void Awake()
    {
        if (progressUIRoot != null)
            progressUIRoot.SetActive(false); // UI скрыт до загрузки
    }

    void Start()
    {
        if (theoryButtonObject != null)
            theoryButton = theoryButtonObject.GetComponent<Button>();

        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // 1. Локальные данные
        float localProgress = PlayerPrefs.GetFloat(PlayerPrefsKey, 0f);
        ApplyProgressToUI(localProgress);

        if (progressUIRoot != null)
            progressUIRoot.SetActive(true); // UI появляется сразу

        // 2. Обновление из Firestore
        LoadProgress();
    }

    private async void LoadProgress()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("User not authenticated");
            return;
        }

        var docRef = db.Collection("users").Document(user.UserId);
        var snapshot = await docRef.GetSnapshotAsync();

        float progress = 0f;

        if (snapshot.Exists && snapshot.ContainsField(levelKey))
        {
            object val = snapshot.GetValue<object>(levelKey);
            if (val is long l) progress = l;
            else if (val is double d) progress = (float)d;
            else if (val is float f) progress = f;
        }
        else
        {
            await docRef.SetAsync(new Dictionary<string, object> { { levelKey, progress } }, SetOptions.MergeAll);
        }

        float cachedProgress = PlayerPrefs.GetFloat(PlayerPrefsKey, 0f);
        if (!Mathf.Approximately(progress, cachedProgress))
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
        bool unlocked = progress >= 1f;

        if (theoryButton != null)
            theoryButton.interactable = unlocked;

        if (theoryButtonImage != null)
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

    public static void ClearCachedProgress()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
    }
}
