using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanelManager : MonoBehaviour
{
    [Header("Основные панели")]
    public GameObject levelButtonObject;
    public GameObject levelPanel;
    public Button cancelButton;
    public Button goToLevelButton;

    [Header("Level 1 – Теория")]
    public GameObject AcceptTherory;
    public GameObject TheoryLevel1;
    public Button TheoryCancel;
    public Button TheoryAccept;

    [Header("Level 2")]
    public GameObject level2;
    public GameObject AcceptLevel2;
    public Button Level2Cancel;
    public Button Level2Accept;

    [Header("Level 3")]
    public GameObject level3;
    public GameObject AcceptLevel3;
    public Button Level3Cancel;
    public Button Level3Accept;

    [Header("Профиль")]
    public GameObject profile;

    private void Start()
    {
        // Скрыть все окна
        levelPanel?.SetActive(false);
        AcceptTherory?.SetActive(false);
        AcceptLevel2?.SetActive(false);
        AcceptLevel3?.SetActive(false);

        // Level Panel
        levelButtonObject?.GetComponent<Button>()?.onClick.AddListener(OpenLevelPanel);
        cancelButton?.onClick.AddListener(CloseLevelPanel);
        goToLevelButton?.onClick.AddListener(() => LoadScene("LevelTheory1"));

        // Level 1
        TheoryLevel1?.GetComponent<Button>()?.onClick.AddListener(OpenAcceptTheroryPanel);
        TheoryCancel?.onClick.AddListener(CloseAcceptTheroryPanel);
        TheoryAccept?.onClick.AddListener(() => LoadScene("Theory2"));

        // Level 2
        level2?.GetComponent<Button>()?.onClick.AddListener(OpenAcceptLevel2Panel);
        Level2Cancel?.onClick.AddListener(CloseAcceptLevel2Panel);
        Level2Accept?.onClick.AddListener(() => LoadScene("LevelTheory2"));

        // Level 3
        level3?.GetComponent<Button>()?.onClick.AddListener(OpenAcceptLevel3Panel);
        Level3Cancel?.onClick.AddListener(CloseAcceptLevel3Panel);
        Level3Accept?.onClick.AddListener(() => LoadScene("LevelTheory3"));

        // Профиль
        profile?.GetComponent<Button>()?.onClick.AddListener(() => LoadScene("Profile"));
    }

    // Панель уровней
    public void OpenLevelPanel() => levelPanel?.SetActive(true);
    public void CloseLevelPanel() => levelPanel?.SetActive(false);

    // Level 1
    public void OpenAcceptTheroryPanel() => AcceptTherory?.SetActive(true);
    public void CloseAcceptTheroryPanel() => AcceptTherory?.SetActive(false);

    // Level 2
    public void OpenAcceptLevel2Panel() => AcceptLevel2?.SetActive(true);
    public void CloseAcceptLevel2Panel() => AcceptLevel2?.SetActive(false);

    // Level 3
    public void OpenAcceptLevel3Panel() => AcceptLevel3?.SetActive(true);
    public void CloseAcceptLevel3Panel() => AcceptLevel3?.SetActive(false);

    // Переход на сцену
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
