using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TabChecker24: MonoBehaviour
{
    public static TabChecker24 Instance;

    public Image tabImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;
    public Sprite defaultSprite;

    public GameObject winPanel;
    public GameObject descriptor;

    public Button confirmButton;
    public Button homeButton;

    private PaletteDrag4 currentPalette;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(Confirm);
        homeButton.onClick.AddListener(GoHome);

        winPanel.SetActive(false);
        descriptor.SetActive(false);
    }

    public void SetPalette(PaletteDrag4 palette)
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
}
