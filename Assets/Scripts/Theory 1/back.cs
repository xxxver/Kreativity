using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject mainPanel;   // Главная панель, которую нужно показать
    public GameObject panelToHide; // Панель, которую нужно скрыть

    // Метод для возвращения на главную панель
    public void OnBackButtonClick()
    {
        // Скрываем текущую панель
        if (panelToHide != null)
        {
            panelToHide.SetActive(false);
        }

        // Показываем главную панель
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }
    }
}
