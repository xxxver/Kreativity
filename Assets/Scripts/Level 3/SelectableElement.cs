using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableElement : MonoBehaviour, IPointerClickHandler
{
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Sprite trueSprite;
    public Sprite falseSprite;

    public bool isCorrectAnswer = false;
    public GameObject wrongDescriptor; // может быть пустым для правильного варианта

    private Image image;
    private static SelectableElement selected;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (wrongDescriptor != null)
        {
            wrongDescriptor.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selected != null && selected != this)
        {
            selected.ResetToDefault();
            selected.HideDescriptor();
        }

        selected = this;
        image.sprite = selectedSprite;
    }

    public void ResetToDefault()
    {
        image.sprite = defaultSprite;
    }

    public static SelectableElement GetSelected()
    {
        return selected;
    }

    public void ShowCorrect()
    {
        image.sprite = trueSprite;
    }

    public void ShowIncorrect()
    {
        image.sprite = falseSprite;
        if (wrongDescriptor != null)
        {
            wrongDescriptor.SetActive(true);
        }
    }

    public void HideDescriptor()
    {
        if (wrongDescriptor != null)
        {
            wrongDescriptor.SetActive(false);
        }
    }
}
