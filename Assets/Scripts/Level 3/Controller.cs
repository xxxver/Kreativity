using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectionController : MonoBehaviour
{
    public Button confirmButton;
    public GameObject winPanel;

    public Button goHomeButton;
    public Button nextLevelButton;

   private void Start()
{
    winPanel.SetActive(false);

    string sceneName = SceneManager.GetActiveScene().name;

    if (sceneName == "Level3-1")
    {
        confirmButton.onClick.AddListener(ConfirmSelection);
        nextLevelButton.onClick.AddListener(() => SceneManager.LoadScene("Level3-2"));
    }
    else if (sceneName == "Level3-2")
    {
        confirmButton.onClick.AddListener(ConfirmSelection);
        nextLevelButton.onClick.AddListener(() => SceneManager.LoadScene("Level3-3"));
    }
    else if (sceneName == "Level3-3")
    {
         confirmButton.onClick.AddListener(ConfirmSelection);
        nextLevelButton.onClick.AddListener(() => SceneManager.LoadScene("Home"));
    }

    goHomeButton.onClick.AddListener(() => SceneManager.LoadScene("Home"));
}
    private void ConfirmSelection()
    {
        SelectableElement selected = SelectableElement.GetSelected();
        if (selected != null)
        {
            if (selected.isCorrectAnswer)
            {
                selected.ShowCorrect();
                StartCoroutine(ShowPanelAfterDelay());
            }
            else
            {
                selected.ShowIncorrect();
            }
        }
    }

    private IEnumerator ShowPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        winPanel.SetActive(true);
    }
}
