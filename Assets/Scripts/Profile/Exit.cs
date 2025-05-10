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

    void Start()
    {
        profileButton.onClick.AddListener(GoToProfile);
        logoutButton.onClick.AddListener(Logout);
        toggleButton.onClick.AddListener(ToggleSettingsPanel);

        // Initially hide the settings panel
        settingsPanel.SetActive(false);
    }

    private void GoToProfile()
    {
        SceneManager.LoadScene("Profile");
    }

    private void Logout()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();

        SceneManager.LoadScene("Auth");
    }

    private void ToggleSettingsPanel()
    {
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            if (!IsPointerOverUIObject())
            {
                settingsPanel.SetActive(false);
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
