using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TabChecker : MonoBehaviour
{
    public static TabChecker Instance;

    public Image tabImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;
    public Sprite defaultSprite;  // Дефолтный спрайт
    public GameObject winPanel;  // WinPanel
    public GameObject descriptor;  // Дескриптор
    public Button confirmButton;

    public Button homeButton;
    public Button nextLevelButton;

    private PaletteDrag currentPalette;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        homeButton.onClick.AddListener(GoHome);
        nextLevelButton.onClick.AddListener(GoToNextLevel);
        confirmButton.onClick.AddListener(Confirm);
        winPanel.SetActive(false); // Скрыть WinPanel изначально
        descriptor.SetActive(false); // Скрыть дескриптор изначально
    }

    public void SetPalette(PaletteDrag palette)
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
            tabImage.sprite = correctSprite; // Успешный спрайт
            StartCoroutine(ShowWinPanelAfterDelay()); // Запускаем корутину с задержкой
        }
        else
        {
            tabImage.sprite = wrongSprite; // Ошибочный спрайт
            descriptor.SetActive(true); // Показываем дескриптор

            // Возвращаем палитру на стартовую позицию
            currentPalette.ReturnToStart();

            // Запускаем корутину для скрытия дескриптора и восстановления спрайта
            StartCoroutine(HandleWrongPalette());
        }

        currentPalette = null; // Убираем ссылку на текущую палитру
    }

    private IEnumerator ShowWinPanelAfterDelay()
    {
        yield return new WaitForSeconds(2f); // Задержка 2 секунды
        winPanel.SetActive(true); // Показываем WinPanel
    }

    private IEnumerator HandleWrongPalette()
    {
        // Ждем 3 секунды
        yield return new WaitForSeconds(3f);

        // После 3 секунд меняем спрайт обратно на дефолтный
        tabImage.sprite = defaultSprite;

        // Скрываем дескриптор
        descriptor.SetActive(false);
    }
    
      private void GoHome()
    {
        SceneManager.LoadScene("Home"); 
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadScene("Level2-1");
    }
}
