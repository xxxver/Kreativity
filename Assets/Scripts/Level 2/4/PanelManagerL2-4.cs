using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // Для корутин

public class ManagerL24 : MonoBehaviour
{
    // Панель и кнопки
    public GameObject panel;  // Панель, которую нужно открыть/закрыть
    public Button actionButton; // Кнопка для перехода на сцену "LevelTheory1"
    public Button extraOpenButton1; // Кнопка для открытия панели
    public Button extraOpenButton2; // Кнопка для открытия панели
    public Button closeButton; // Кнопка для закрытия панели
    public Button confirmButton; // Кнопка для подтверждения перехода на сцену "Home"

    private void Start()
    {
        // Панель скрыта при старте
        panel.SetActive(false);

        // Привязываем методы к кнопкам
        actionButton.onClick.AddListener(ShowPanelAndGoToLevelTheory1);  // Показываем панель и переходим на сцену "LevelTheory1"
        extraOpenButton1.onClick.AddListener(OpenPanel);     // Открыть панель
        extraOpenButton2.onClick.AddListener(OpenPanel);     // Открыть панель
        closeButton.onClick.AddListener(ClosePanel);         // Закрыть панель
        confirmButton.onClick.AddListener(ConfirmAndGoHome); // Переход на сцену "Home"
    }

    // Метод для показа панели и перехода на сцену "LevelTheory1"
    private void ShowPanelAndGoToLevelTheory1()
    {
        // Сначала открываем панель
        OpenPanel();

        // Запускаем корутину для ожидания закрытия панели перед переходом
        StartCoroutine(WaitForPanelCloseAndGoToLevelTheory1());
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
        // Не переходим на другую сцену, оставляем пользователя на текущей
    }

    // Метод для подтверждения перехода на сцену "Home"
    private void ConfirmAndGoHome()
    {
        // Переход на сцену "Home"
        SceneManager.LoadScene("Home");
    }

    // Корутин для ожидания закрытия панели и перехода на сцену "LevelTheory1"
    private IEnumerator WaitForPanelCloseAndGoToLevelTheory1()
    {
        // Ожидаем, пока панель не будет скрыта
        while (panel.activeSelf)
        {
            yield return null;  // Ожидаем один кадр
        }

        // После того как панель закрыта, переходим на сцену "Home"
        // Убираем переход на другую сцену, так как остаемся на текущей
    }
}
