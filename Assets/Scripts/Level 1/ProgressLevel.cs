using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
public Button cancelButton; // Кнопка Cancel на панели Win

void Start()
{
    cancelButton.onClick.AddListener(OnCancelClicked);
}

private void OnCancelClicked()
{
    float currentProgress = PlayerPrefs.GetFloat("LevelProgress", 0f);

    // Обновляем прогресс только если он меньше 0.5
    if (currentProgress < 0.5f)
    {
        PlayerPrefs.SetFloat("LevelProgress", 0.5f);
        PlayerPrefs.Save();
    }

    SceneManager.LoadScene("Home");
}
}