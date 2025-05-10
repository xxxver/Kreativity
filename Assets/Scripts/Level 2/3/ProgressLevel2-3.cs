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
    float currentProgress = PlayerPrefs.GetFloat("LevelProgress2", 0f);

    // Сохраняем прогресс 0.6, только если он меньше 0.8
    if (currentProgress < 0.8f)
    {
        PlayerPrefs.SetFloat("LevelProgress2", 0.6f);
        PlayerPrefs.Save();
    }

    SceneManager.LoadScene("Home");
}

}
