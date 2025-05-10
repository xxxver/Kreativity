using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBarl22 : MonoBehaviour
{
    public Button cancelButton;  // Кнопка Cancel на панели Win

    void Start()
{
    cancelButton.onClick.AddListener(OnCancelClicked);
}

private void OnCancelClicked()
{
    float currentProgress = PlayerPrefs.GetFloat("LevelProgress2", 0f);

    // Сохраняем прогресс только если текущий прогресс < 0.6
    if (currentProgress < 0.6f)
    {
        PlayerPrefs.SetFloat("LevelProgress2", 0.4f);
        PlayerPrefs.Save();
    }

    SceneManager.LoadScene("Home");
}

}
