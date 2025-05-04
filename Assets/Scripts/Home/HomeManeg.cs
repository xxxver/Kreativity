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

    public GameObject level2;
    public GameObject AcceptLevel2;
    public Button Level2Cancel;
    public Button Level2Accept;

    // 🔹 Новые объекты для Level3
    public GameObject level3;
    public GameObject AcceptLevel3;
    public Button Level3Cancel;
    public Button Level3Accept;

    private void Start()
    {
        levelPanel.SetActive(false);
        AcceptTherory.SetActive(false);
        AcceptLevel2.SetActive(false);
        AcceptLevel3.SetActive(false); // Скрываем подтверждение Level3 при старте

        levelButtonObject.GetComponent<Button>().onClick.AddListener(OpenLevelPanel);
        cancelButton.onClick.AddListener(CloseLevelPanel);
        goToLevelButton.onClick.AddListener(() => LoadScene("LevelTheory1"));

        TheoryLevel1.GetComponent<Button>().onClick.AddListener(OpenAcceptTheroryPanel);
        TheoryCancel.onClick.AddListener(CloseAcceptTheroryPanel);
        TheoryAccept.onClick.AddListener(() => LoadScene("Theory1"));

        level2.GetComponent<Button>().onClick.AddListener(OpenAcceptLevel2Panel);
        Level2Cancel.onClick.AddListener(CloseAcceptLevel2Panel);
        Level2Accept.onClick.AddListener(() => LoadScene("LevelTheory2"));

        // 🔹 Обработка нажатия для level3
        level3.GetComponent<Button>().onClick.AddListener(OpenAcceptLevel3Panel);
        Level3Cancel.onClick.AddListener(CloseAcceptLevel3Panel);
        Level3Accept.onClick.AddListener(() => LoadScene("LevelTheory3"));
    }

    public void OpenLevelPanel() => levelPanel.SetActive(true);
    public void CloseLevelPanel() => levelPanel.SetActive(false);

    public void OpenAcceptTheroryPanel() => AcceptTherory.SetActive(true);
    public void CloseAcceptTheroryPanel() => AcceptTherory.SetActive(false);

    public void OpenAcceptLevel2Panel() => AcceptLevel2.SetActive(true);
    public void CloseAcceptLevel2Panel() => AcceptLevel2.SetActive(false);

    // 🔹 Методы для подтверждения Level3
    public void OpenAcceptLevel3Panel() => AcceptLevel3.SetActive(true);
    public void CloseAcceptLevel3Panel() => AcceptLevel3.SetActive(false);

    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
}
