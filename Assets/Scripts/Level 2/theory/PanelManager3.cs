using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Theory2_4Manager : MonoBehaviour
{
    public GameObject panel;
    public Button openButton;
    public Button closeButton;
    public Button actionButton;
    public Button nextSceneButton;
    public Button extraOpenButton1;
    public Button extraOpenButton2;

    private void Start()
    {
        panel.SetActive(false);

        // Кнопка openButton теперь выполняет переход назад на сцену "Theory2-2"
        openButton.onClick.AddListener(() => LoadScene("Theory2-2"));

        closeButton.onClick.AddListener(ClosePanel);
        actionButton.onClick.AddListener(() => LoadScene("Home"));
        nextSceneButton.onClick.AddListener(() => LoadScene("Home"));

        extraOpenButton1.onClick.AddListener(OpenPanel);
        extraOpenButton2.onClick.AddListener(OpenPanel);
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
