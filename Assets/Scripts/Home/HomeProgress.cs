using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Подключаем TextMeshPro

public class ProgressBarManager : MonoBehaviour
{
    public Slider progressBar;  // Прогрессбар на сцене Home
    public TMP_Text progressText;   // TextMeshPro для отображения процента

    void Start()
    {
        // Загружаем прогресс из PlayerPrefs (по умолчанию 0, если прогресс не сохранен)
        float progress = PlayerPrefs.GetFloat("LevelProgress", 0f);

        // Устанавливаем значение прогрессбара
        progressBar.value = progress;  

        // Обновляем текст с процентом
        UpdateProgressText(progress);
    }

    // Метод для обновления текста прогресса
    private void UpdateProgressText(float progress)
    {
        // Преобразуем прогресс в проценты (например, 0.5 -> 50%)
        int percent = Mathf.RoundToInt(progress * 100);
        progressText.text = $"{percent}%";  // Обновляем текст
    }
}
