using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Добавьте эту строку
using System.Collections;

public class TabDropController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image[] images; // Массив изображений, которые будут показываться

    private void Start()
    {
        foreach (var image in images)
        {
            image.enabled = false; // Скрыть изображения изначально
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered tab");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited tab");
    }

    public void OnIconDropped(GameObject icon)
    {
        Debug.Log("Icon dropped on tab");

        // Показываем соответствующее изображение
        ShowImageForIcon(icon);

        // Анимация изображения
        StartCoroutine(AnimateImage(images[GetIconIndex(icon)]));
    }

    void ShowImageForIcon(GameObject icon)
    {
        int index = GetIconIndex(icon);
        if (index >= 0 && index < images.Length)
        {
            images[index].enabled = true;
        }
    }

    int GetIconIndex(GameObject icon)
    {
        // Здесь реализуйте логику получения индекса иконки
        // Например, используйте имя иконки или другой уникальный идентификатор
        return 0;
    }

    private IEnumerator AnimateImage(Image image)
    {
        float duration = 1.0f; // Продолжительность анимации
        float startScale = 1.0f; // Начальный размер
        float endScale = 1.2f; // Конечный размер

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            image.transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, t);
            yield return null;
        }

        // Возвращаемся к исходному размеру
        yield return new WaitForSeconds(1.0f);
        elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            image.transform.localScale = Vector3.Lerp(Vector3.one * endScale, Vector3.one * startScale, t);
            yield return null;
        }
    }
}