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

        confirmButton.onClick.AddListener(ConfirmSelection);
        goHomeButton.onClick.AddListener(() => SceneManager.LoadScene("Home"));
        nextLevelButton.onClick.AddListener(GoToNextLevel);
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

    private void GoToNextLevel()
    {
        Debug.Log("Следующий уровень пока не реализован");
    }
}
