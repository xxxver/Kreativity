using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBar2 : MonoBehaviour
{
    public Button cancelButton;  // Кнопка Cancel на панели Win

    void Start()
    {
        // Привязываем метод к кнопке Cancel
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    // Метод, вызываемый при нажатии на Cancel
    private void OnCancelClicked()
    {
        // Сохраняем прогресс как 50% (0.5)
        PlayerPrefs.SetFloat("LevelProgress", 1.0f);
        PlayerPrefs.Save();

        // Переход на сцену Home
        SceneManager.LoadScene("Home");
    }
}
