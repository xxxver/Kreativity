using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBarl2 : MonoBehaviour
{
    public Button cancelButton;  // Кнопка Cancel на панели Win

    void Start()
{
    cancelButton.onClick.AddListener(OnCancelClicked);
}

private void OnCancelClicked()
{
    float currentProgress = PlayerPrefs.GetFloat("LevelProgress2", 0f);

    // Устанавливаем 0.2, только если прогресс меньше 0.4
    if (currentProgress < 0.4f)
    {
        PlayerPrefs.SetFloat("LevelProgress2", 0.2f);
        PlayerPrefs.Save();
    }

    SceneManager.LoadScene("Home");
}

}
