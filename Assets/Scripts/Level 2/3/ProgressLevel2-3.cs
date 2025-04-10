using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBarl23 : MonoBehaviour
{
    public Button cancelButton;  // Кнопка Cancel на панели Win

    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    private void OnCancelClicked()
    {
        // Сохраняем прогресс как 50% (0.5) с ключом LevelProgress2
        PlayerPrefs.SetFloat("LevelProgress2", 0.6f);
        PlayerPrefs.Save();

        // Переход на сцену Home
        SceneManager.LoadScene("Home");
    }
}
