using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanelManager : MonoBehaviour
{
    public GameObject levelButtonObject;
    public GameObject levelPanel;
    public Button cancelButton;
    public Button goToLevelButton;

    public GameObject AcceptTherory;
    public GameObject TheoryLevel1;
    public Button TheoryCancel;
    public Button TheoryAccept;

    public GameObject level2; // 🔹 Новый GameObject для перехода на Level2

    private void Start()
    {
        levelPanel.SetActive(false);
        AcceptTherory.SetActive(false);

        levelButtonObject.GetComponent<Button>().onClick.AddListener(OpenLevelPanel);
        cancelButton.onClick.AddListener(CloseLevelPanel);
        goToLevelButton.onClick.AddListener(() => LoadScene("LevelTheory1"));

        TheoryLevel1.GetComponent<Button>().onClick.AddListener(OpenAcceptTheroryPanel);
        TheoryCancel.onClick.AddListener(CloseAcceptTheroryPanel);
        TheoryAccept.onClick.AddListener(() => LoadScene("Theory1"));

        // 🔹 Обработка нажатия на GameObject level2 (должен содержать компонент Button)
        level2.GetComponent<Button>().onClick.AddListener(() => LoadScene("Level2"));
    }

    public void OpenLevelPanel()
    {
        levelPanel.SetActive(true);
    }

    public void CloseLevelPanel()
    {
        levelPanel.SetActive(false);
    }

    public void OpenAcceptTheroryPanel()
    {
        AcceptTherory.SetActive(true);
    }

    public void CloseAcceptTheroryPanel()
    {
        AcceptTherory.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
