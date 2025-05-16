    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Firebase.Auth;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;

    public class Settings : MonoBehaviour
    {
        public Button profileButton;
        public Button logoutButton;
        public Button toggleButton;
        public GameObject settingsPanel;

        private FirebaseAuth auth;

        void Start()
        {
            profileButton.onClick.AddListener(GoToProfile);
            logoutButton.onClick.AddListener(Logout);
            toggleButton.onClick.AddListener(ToggleSettingsPanel);

            settingsPanel.SetActive(false);
            auth = FirebaseAuth.DefaultInstance;
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

            // 🧹 Очищаем кэш прогресса
            ProgressBarManager.ClearCachedProgress();
            ProgressBarManager2.ClearCachedProgress();
            ProgressBarManager3.ClearCachedProgress();



            UserData.Instance.ClearUserDataAndReloadScene("Auth");
            
        }

        private void ToggleSettingsPanel()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        void Update()
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
