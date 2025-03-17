using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Для работы с UI элементами

public class LevelPanelManager : MonoBehaviour
{
    public GameObject levelButtonObject;  // GameObject, который активирует панель
    public GameObject levelPanel;  // Панель с кнопками
    public Button cancelButton;  // Кнопка для отмены (закрывает панель)
    public Button goToLevelButton;  // Кнопка для перехода на сцену

    private void Start()
    {
        // Панель скрыта при старте
        levelPanel.SetActive(false);

        // Привязываем методы к кнопкам
        levelButtonObject.GetComponent<Button>().onClick.AddListener(OpenLevelPanel);  // Открытие панели
        cancelButton.onClick.AddListener(CloseLevelPanel);  // Закрытие панели
        goToLevelButton.onClick.AddListener(() => LoadScene("LevelTheory1"));  // Переход на сцену "LevelScene"
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

    // Метод для перехода на другую сцену
    public void LoadScene(string LevelTheory1)
    {
        SceneManager.LoadScene(LevelTheory1);  // Переход на сцену с заданным названием
    }
}
