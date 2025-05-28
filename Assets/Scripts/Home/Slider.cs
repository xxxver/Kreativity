using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ReadOnlySlider : MonoBehaviour
{
    private void Awake()
    {
        Slider slider = GetComponent<Slider>();
        slider.interactable = false;

        CanvasGroup canvasGroup = slider.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = slider.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
    }
}
