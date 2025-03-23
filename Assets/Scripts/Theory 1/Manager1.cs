using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцен

public class ManagerT1 : MonoBehaviour
{
    public GameObject mainPanel; // Главная панель
    public GameObject[] panels;  // Массив дополнительных панелей
    public GameObject backButton;  // Кнопка Back
    public GameObject activePanel; // Панель Active

    void Start()
    {
          activePanel.SetActive(false);
        // Скрываем все панели, кроме mainPanel
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false); // Скрываем все дополнительные панели
        }
        mainPanel.SetActive(true); // Показываем главную панель
    }

    // Метод для показа главной панели и скрытия остальных
    public void OnBackButtonClick()
    {
        if (mainPanel.activeSelf)
        {
            // Если mainPanel не скрыта, показываем activePanel
            if (activePanel != null)
            {
                activePanel.SetActive(true); // Показываем панель Active
            }
        }
        else
        {
            // Если mainPanel скрыта, показываем её и скрываем все остальные панели
            foreach (GameObject panel in panels)
            {
                panel.SetActive(false); // Скрываем все дополнительные панели
            }

            mainPanel.SetActive(true); // Показываем главную панель
        }
    }

    // Метод для перехода на другую сцену
    public void GoToScene(string Home)
    {
        SceneManager.LoadScene("Home");
    }

    // Метод для скрытия панели active и возврата на mainPanel
    public void HideActivePanel()
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false); // Скрываем панель Active
        }
        mainPanel.SetActive(true); // Показываем главную панель
    }
}
