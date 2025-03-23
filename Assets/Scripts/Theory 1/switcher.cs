using UnityEngine;
using UnityEngine.EventSystems;  // Для работы с кликами

public class Switcher : MonoBehaviour, IPointerClickHandler
{
    public GameObject mainPanel;  // Главная панель, которую нужно показать
    public GameObject panelToHide; // Панель, которую нужно скрыть

    // Обработчик клика по кнопке
    public void OnPointerClick(PointerEventData eventData)
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
