using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager1 : MonoBehaviour
{
    // Панель и кнопки
    public GameObject panel;  // Панель, которую нужно открыть/закрыть
    public Button actionButton; // Кнопка для перехода на сцену "PanelLevel1"
    public Button extraOpenButton1; // Кнопка для открытия панели
    public Button extraOpenButton2; // Кнопка для открытия панели
    public Button closeButton; // Кнопка для закрытия панели
    public Button confirmButton; // Кнопка для подтверждения перехода на сцену "Home"

    private void Start()
    {
        // Панель скрыта при старте
        panel.SetActive(false);

        // Привязываем методы к кнопкам
        actionButton.onClick.AddListener(GoToLevelTheory1);  // Переход на сцену "PanelLevel1"
        extraOpenButton1.onClick.AddListener(OpenPanel);     // Открыть панель
        extraOpenButton2.onClick.AddListener(OpenPanel);     // Открыть панель
        closeButton.onClick.AddListener(ClosePanel);         // Закрыть панель
        confirmButton.onClick.AddListener(ConfirmAndGoHome); // Переход на сцену "Home"
    }

    // Метод для перехода на сцену "PanelLevel1"
    private void GoToLevelTheory1()
    {
        // Переход на сцену "LevelTheory1" без открытия панели
        SceneManager.LoadScene("Level1");
    }

    // Метод для открытия панели
    private void OpenPanel()
    {
        panel.SetActive(true);  // Показываем панель
    }

    // Метод для закрытия панели
    private void ClosePanel()
    {
        panel.SetActive(false);  // Скрываем панель
    }

    // Метод для подтверждения перехода на сцену "Home"
    private void ConfirmAndGoHome()
    {
        // Переход на сцену "Home"
        SceneManager.LoadScene("Home");
    }
}
