using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject levelPanel;
    public GameObject theoryPanel;
    public Button levelButton;
    public Button theoryButton;
    public Slider progressBar;  // Прогрессбар на главной сцене

    void Start()
    {

         float progress = PlayerPrefs.GetFloat("LevelProgress", 0f);
        progressBar.value = progress;


        levelButton.onClick.AddListener(ShowLevelPanel);
        theoryButton.onClick.AddListener(ShowTheoryPanel);
        theoryPanel.SetActive(false); // Изначально скрываем панель теории
    }

    void ShowLevelPanel()
    {
        levelPanel.SetActive(true);
        theoryPanel.SetActive(false);
    }

    void ShowTheoryPanel()
    {
        theoryPanel.SetActive(true);
        levelPanel.SetActive(false);
    }
}
