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
    public GameObject TheoryLevel2;
    public Button Level2Cancel;
    public Button Level2Accept;
    public GameObject AcceptTheoryLevel2;
    public Button Theory2Cancel;
    public Button Theory2Accept;

    [Header("Level 3")]
    public GameObject level3;
    public GameObject AcceptLevel3;
    public GameObject TheoryLevel3;
    public Button Level3Cancel;
    public Button Level3Accept;
    public GameObject AcceptTheoryLevel3;
    public Button Theory3Cancel;
    public Button Theory3Accept;

    [Header("Level 4")]
    public GameObject level4;
    public GameObject subPanel;
    public Button subPanelCloseButton;

    [Header("Профиль")]
    public GameObject profile;

    private void Start()
    {
        // Скрыть все окна
        levelPanel?.SetActive(false);
        AcceptTherory?.SetActive(false);
        AcceptLevel2?.SetActive(false);
        AcceptLevel3?.SetActive(false);
        AcceptTheoryLevel2?.SetActive(false);
        AcceptTheoryLevel3?.SetActive(false);
        subPanel?.SetActive(false);

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
        TheoryLevel2?.GetComponent<Button>()?.onClick.AddListener(OpenAcceptTheoryLevel2Panel);
        Theory2Cancel?.onClick.AddListener(CloseAcceptTheoryLevel2Panel);
        Theory2Accept?.onClick.AddListener(() => LoadScene("Theory2-1"));

        // Level 3
        level3?.GetComponent<Button>()?.onClick.AddListener(OpenAcceptLevel3Panel);
        Level3Cancel?.onClick.AddListener(CloseAcceptLevel3Panel);
        Level3Accept?.onClick.AddListener(() => LoadScene("LevelTheory3"));
        TheoryLevel3?.GetComponent<Button>()?.onClick.AddListener(OpenAcceptTheoryLevel3Panel);
        Theory3Cancel?.onClick.AddListener(CloseAcceptTheoryLevel3Panel);
        Theory3Accept?.onClick.AddListener(() => LoadScene("Theory3-1"));

        // Level 4
        level4?.GetComponent<Button>()?.onClick.AddListener(OpenSubPanel);
        subPanelCloseButton?.onClick.AddListener(CloseSubPanel);

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
    public void OpenAcceptTheoryLevel2Panel() => AcceptTheoryLevel2?.SetActive(true);
    public void CloseAcceptTheoryLevel2Panel() => AcceptTheoryLevel2?.SetActive(false);

    // Level 3
    public void OpenAcceptLevel3Panel() => AcceptLevel3?.SetActive(true);
    public void CloseAcceptLevel3Panel() => AcceptLevel3?.SetActive(false);
    public void OpenAcceptTheoryLevel3Panel() => AcceptTheoryLevel3?.SetActive(true);
    public void CloseAcceptTheoryLevel3Panel() => AcceptTheoryLevel3?.SetActive(false);

    // Level 4
    public void OpenSubPanel() => subPanel?.SetActive(true);
    public void CloseSubPanel() => subPanel?.SetActive(false);

    // Переход на сцену
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
