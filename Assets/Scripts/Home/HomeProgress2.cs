using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarManager2 : MonoBehaviour
{
    public Slider progressBar;  // Прогрессбар на сцене Home
    public TMP_Text progressText;   // TextMeshPro для отображения процента
    public GameObject theoryButtonObject; // GameObject кнопки теории
    public GameObject theoryPanel; // Панель с теорией (для скрытия компонентов внутри)
    public Image theoryButtonImage; // Image кнопки теории (для смены спрайта)
    
    public Sprite theoryBlockedSprite; // Спрайт для заблокированного состояния
    public Sprite theoryActiveSprite;  // Спрайт для активного состояния

    private Button theoryButton; // Компонент Button

    void Start()
    {
        theoryButton = theoryButtonObject.GetComponent<Button>();

        // ЗАГРУЖАЕМ ПРОГРЕСС (ключ с 2)
        float progress = PlayerPrefs.GetFloat("LevelProgress2", 0f);

        progressBar.value = progress;
        UpdateProgressText(progress);
        CheckUnlockTheory(progress);
    }

    private void UpdateProgressText(float progress)
    {
        int percent = Mathf.RoundToInt(progress * 100);
        progressText.text = $"{percent}%";
    }

    private void CheckUnlockTheory(float progress)
    {
        if (progress >= 1f)
        {
            theoryButton.interactable = true;
            theoryButtonImage.sprite = theoryActiveSprite;
            ShowTheoryPanel(true);
        }
        else
        {
            theoryButton.interactable = false;
            theoryButtonImage.sprite = theoryBlockedSprite;
            ShowTheoryPanel(false);
        }
    }

    private void ShowTheoryPanel(bool show)
    {
        foreach (Transform child in theoryPanel.transform)
        {
            child.gameObject.SetActive(show);
        }
    }
}
