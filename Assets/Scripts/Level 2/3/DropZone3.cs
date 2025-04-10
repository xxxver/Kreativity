using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TabChecker23 : MonoBehaviour
{
    public static TabChecker23 Instance;

    public Image tabImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;
    public Sprite defaultSprite;

    public GameObject winPanel;
    public GameObject descriptor;

    public Button confirmButton;
    public Button homeButton;
    public Button nextLevelButton;

    private PaletteDrag3 currentPalette;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(Confirm);
        homeButton.onClick.AddListener(GoHome);
        nextLevelButton.onClick.AddListener(GoToNextLevel);

        winPanel.SetActive(false);
        descriptor.SetActive(false);
    }

    public void SetPalette(PaletteDrag3 palette)
    {
        currentPalette = palette;
    }

    public void ClearPalette()
    {
        currentPalette = null;
    }

    private void Confirm()
    {
        if (currentPalette == null) return;

        if (currentPalette.isCorrectPalette)
        {
            tabImage.sprite = correctSprite;
            StartCoroutine(ShowWinPanelAfterDelay());
        }
        else
        {
            tabImage.sprite = wrongSprite;
            descriptor.SetActive(true);

            currentPalette.ReturnToStart();

            StartCoroutine(HandleWrongPalette());
        }

        currentPalette = null;
    }

    private IEnumerator ShowWinPanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        winPanel.SetActive(true);
    }

    private IEnumerator HandleWrongPalette()
    {
        yield return new WaitForSeconds(3f);

        tabImage.sprite = defaultSprite;
        descriptor.SetActive(false);
    }

    private void GoHome()
    {
        SceneManager.LoadScene("Home"); 
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadScene("Level2-3");
    }
}
