using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressBarl3_2 : MonoBehaviour
{
    public Button cancelButton;  // Кнопка Cancel на панели Win

    void Start()
{
    cancelButton.onClick.AddListener(OnCancelClicked);
}

private void OnCancelClicked()
{
    float currentProgress = PlayerPrefs.GetFloat("LevelProgress3", 0f);

    if (currentProgress < 1f)
    {
        PlayerPrefs.SetFloat("LevelProgress3", 0.66f);
        PlayerPrefs.Save();
    }

    SceneManager.LoadScene("Home");
}

}
