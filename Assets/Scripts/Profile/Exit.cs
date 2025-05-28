using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class Settings : MonoBehaviour
{
    public Button profileButton;
    public Button logoutButton;
    public Button toggleButton;
    public GameObject settingsPanel;
    public TMP_Text subscriptionDateLabel;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private void Start()
    {
        profileButton.onClick.AddListener(GoToProfile);
        logoutButton.onClick.AddListener(Logout);
        toggleButton.onClick.AddListener(ToggleSettingsPanel);

        settingsPanel.SetActive(false);

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (subscriptionDateLabel != null)
            subscriptionDateLabel.gameObject.SetActive(false);

        LoadSubscriptionStatus();
    }

    private async void LoadSubscriptionStatus()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null || db == null)
            return;

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists && snapshot.ContainsField("subscriptionEndTime"))
        {
            Timestamp timestamp = snapshot.GetValue<Timestamp>("subscriptionEndTime");
            DateTime endTime = timestamp.ToDateTime();
            DateTime now = DateTime.UtcNow;

            bool isActive = endTime > now;
            if (subscriptionDateLabel != null)
            {
                if (isActive)
                {
                    subscriptionDateLabel.gameObject.SetActive(true);
                    subscriptionDateLabel.text = $"Дата окончания: {endTime.ToLocalTime():dd.MM.yyyy}";
                }
                else
                {
                    subscriptionDateLabel.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (subscriptionDateLabel != null)
                subscriptionDateLabel.gameObject.SetActive(false);
        }
    }

    private void GoToProfile()
    {
        SceneManager.LoadScene("Profile");
    }

    private void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
        }

        ProgressBarManager.ClearCachedProgress();
        ProgressBarManager2.ClearCachedProgress();
        ProgressBarManager3.ClearCachedProgress();

        UserData.Instance.ClearUserDataAndReloadScene("Auth");
    }

    private void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            settingsPanel.SetActive(false);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
