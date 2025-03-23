using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarManager : MonoBehaviour
{
    public Slider progressBar;  // Прогрессбар на сцене Home
    public TMP_Text progressText;   // TextMeshPro для отображения процента
    public GameObject theoryButtonObject; // GameObject кнопки теории
    public GameObject theoryPanel; // Панель с теорией (для скрытия компонентов внутри)
    public Image theoryButtonImage; // Image кнопки теории (для смены спрайта)
    
    public Sprite theoryBlockedSprite; // Спрайт для заблокированного состояния
    public Sprite theoryActiveSprite;  // Спрайт для активного состояния

    private Button theoryButton; // Компонент Button

    void Start()
    {
        // Получаем компонент Button с объекта теории
        theoryButton = theoryButtonObject.GetComponent<Button>();

        // Загружаем прогресс из PlayerPrefs (по умолчанию 0, если не сохранен)
        float progress = PlayerPrefs.GetFloat("LevelProgress", 0f);

        // Устанавливаем значение прогрессбара
        progressBar.value = progress;

        // Обновляем текст с процентом
        UpdateProgressText(progress);

        // Проверяем, нужно ли активировать кнопку теории
        CheckUnlockTheory(progress);
    }

    // Метод для обновления текста прогресса
    private void UpdateProgressText(float progress)
    {
        int percent = Mathf.RoundToInt(progress * 100);
        progressText.text = $"{percent}%";
    }

    // Метод для проверки и активации кнопки теории
    private void CheckUnlockTheory(float progress)
    {
        if (progress >= 1f) // 100%
        {
            theoryButton.interactable = true;  // Активируем кнопку
            theoryButtonImage.sprite = theoryActiveSprite; // Сменить спрайт на активный
            ShowTheoryPanel(true); // Показать панель с теорией (если она скрыта)
        }
        else
        {
            theoryButton.interactable = false; // Отключаем кнопку
            theoryButtonImage.sprite = theoryBlockedSprite; // Сменить спрайт на заблокированный
            ShowTheoryPanel(false); // Скрыть панель с теорией
        }
    }

    // Метод для скрытия/показывания панели теории
    private void ShowTheoryPanel(bool show)
    {
        // Здесь мы скрываем или показываем компоненты в панели теории
        foreach (Transform child in theoryPanel.transform)
        {
            child.gameObject.SetActive(show); // Скрыть/показать все дочерние объекты
        }
    }
}
