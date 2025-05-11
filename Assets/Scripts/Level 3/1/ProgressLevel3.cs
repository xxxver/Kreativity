using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBarl3_3 : MonoBehaviour
{
    public Button cancelButton;  // Кнопка Cancel на панели Win

    void Start()
{
    cancelButton.onClick.AddListener(OnCancelClicked);
}

private void OnCancelClicked()
{
    float currentProgress = PlayerPrefs.GetFloat("LevelProgress3", 0f);

    if (currentProgress < 0.66f)
    {
        PlayerPrefs.SetFloat("LevelProgress3", 0.33f);
        PlayerPrefs.Save();
    }

    SceneManager.LoadScene("Home");
}

}
