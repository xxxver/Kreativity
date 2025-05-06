using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцен
using UnityEngine.UI; // Для работы с элементами интерфейса

public class ManagerT1 : MonoBehaviour
{
    public GameObject mainPanel; // Главная панель
    public GameObject[] panels;  // Массив дополнительных панелей
    public GameObject backButton;  // Кнопка Back
    public GameObject activePanel; // Панель Active
    public GameObject nextButton;  // Кнопка Next
    public GameObject confirmButton;  // Кнопка Confirm
    public GameObject cancelButton;  // Кнопка Cancel

    void Start()
    {
        activePanel.SetActive(false);
        // Скрываем все панели, кроме mainPanel
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false); // Скрываем все дополнительные панели
        }
        mainPanel.SetActive(true); // Показываем главную панель

        // Добавляем слушатели для кнопок
        if (nextButton != null)
        {
            nextButton.GetComponent<Button>().onClick.AddListener(OnNextButtonClick);
        }
        if (confirmButton != null)
        {
            confirmButton.GetComponent<Button>().onClick.AddListener(OnConfirmButtonClick);
        }
        if (cancelButton != null)
        {
            cancelButton.GetComponent<Button>().onClick.AddListener(OnCancelButtonClick);
        }
    }

    // Метод для показа главной панели и скрытия остальных
    public void OnBackButtonClick()
    {
        if (mainPanel.activeSelf)
        {
            // Если mainPanel не скрыта, переходим на сцену Theory2
            GoToScene("Theory2");
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
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

    // Метод для обработки нажатия на кнопку Next
    public void OnNextButtonClick()
    {
        if (activePanel != null)
        {
            activePanel.SetActive(true); // Показываем панель Active
        }
        mainPanel.SetActive(false); // Скрываем главную панель
    }

    // Метод для обработки нажатия на кнопку Confirm
    public void OnConfirmButtonClick()
    {
        GoToScene("Home");
    }

    // Метод для обработки нажатия на кнопку Cancel
    public void OnCancelButtonClick()
    {
        HideActivePanel();
    }
}
