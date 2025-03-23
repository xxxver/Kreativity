using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Для работы с UI элементами

public class LevelPanelManager : MonoBehaviour
{
    public GameObject levelButtonObject;  // GameObject, который активирует панель
    public GameObject levelPanel;  // Панель с кнопками
    public Button cancelButton;  // Кнопка для отмены (закрывает панель)
    public Button goToLevelButton;  // Кнопка для перехода на сцену
    public GameObject AcceptTherory;  // Панель для действий
    public GameObject TheoryLevel1;  // GameObject для открытия action панели
    public Button TheoryCancel;  // Кнопка для закрытия action панели
    public Button TheoryAccept;  // Кнопка для перехода на другую сцену

    private void Start()
    {
        // Панели скрыты при старте
        levelPanel.SetActive(false);
        AcceptTherory.SetActive(false);

        // Привязываем методы к кнопкам
        levelButtonObject.GetComponent<Button>().onClick.AddListener(OpenLevelPanel);  // Открытие панели
        cancelButton.onClick.AddListener(CloseLevelPanel);  // Закрытие панели
        goToLevelButton.onClick.AddListener(() => LoadScene("LevelTheory1"));  // Переход на сцену "LevelTheory1"
        
        // Привязываем методы для action панели
        TheoryLevel1.GetComponent<Button>().onClick.AddListener(OpenAcceptTheroryPanel);  // Открытие action панели
        TheoryCancel.onClick.AddListener(CloseAcceptTheroryPanel);  // Закрытие action панели
        TheoryAccept.onClick.AddListener(() => LoadScene("Theory1"));  // Переход на сцену "Theory1"
    }

    // Метод для открытия панели
    public void OpenLevelPanel()
    {
        levelPanel.SetActive(true);  // Показываем панель
    }

    // Метод для закрытия панели
    public void CloseLevelPanel()
    {
        levelPanel.SetActive(false);  // Скрываем панель
    }

    // Метод для открытия action панели
    public void OpenAcceptTheroryPanel()
    {
        AcceptTherory.SetActive(true);  // Показываем action панель
    }

    // Метод для закрытия action панели
    public void CloseAcceptTheroryPanel()
    {
        AcceptTherory.SetActive(false);  // Скрываем action панель
    }

    // Метод для перехода на другую сцену
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);  // Переход на сцену с заданным названием
    }
}
