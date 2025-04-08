using UnityEngine;
using UnityEngine.UI;

public class TabChecker : MonoBehaviour
{
    public static TabChecker Instance;

    public Image tabImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;
    public GameObject descriptorPanel;
    public Button confirmButton;

    private PaletteDrag currentPalette;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(Confirm);
        descriptorPanel.SetActive(false);
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

        tabImage.sprite = currentPalette.isCorrectPalette ? correctSprite : wrongSprite;
        descriptorPanel.SetActive(true);

        // Удалить палитру из таба
        Destroy(currentPalette.gameObject);
        currentPalette = null;
    }
}
