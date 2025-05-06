using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Для работы с кнопками

public class Theory1Manager : MonoBehaviour
{
    public GameObject panel;  // Панель, которую нужно открыть/закрыть
    public Button openButton;  // Кнопка для открытия панели
    public Button closeButton; // Кнопка для закрытия панели внутри панели
    public Button actionButton; // Кнопка для перехода на сцену внутри панели
    public Button nextSceneButton; // Кнопка для перехода на следующую сцену
    public Button extraOpenButton1; // Дополнительная кнопка для открытия панели
    public Button extraOpenButton2; // Еще одна дополнительная кнопка для открытия панели

    private void Start()
    {
        // Панель скрыта при старте
        panel.SetActive(false);

        // Привязываем методы к кнопкам
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);
        actionButton.onClick.AddListener(() => LoadScene("Home"));  // Название сцены, на которую мы переходим
        nextSceneButton.onClick.AddListener(() => LoadScene("Theory1"));  // Переход на следующую сцену

        // Привязываем дополнительные кнопки для открытия панели
        extraOpenButton1.onClick.AddListener(OpenPanel);
        extraOpenButton2.onClick.AddListener(OpenPanel);
    }

    // Метод для открытия панели
    public void OpenPanel()
    {
        panel.SetActive(true);  // Показываем панель
    }

    // Метод для закрытия панели
    public void ClosePanel()
    {
        panel.SetActive(false);  // Скрываем панель
    }

    // Метод для перехода на другую сцену
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);  // Переход на сцену с заданным названием
    }
}
