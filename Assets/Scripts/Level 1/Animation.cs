using System.Collections;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    public static ScaleAnimation Instance; // Синглтон для удобного доступа

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Запускает анимацию масштаба для указанного объекта.
    /// </summary>
    /// <param name="target">Целевой объект для анимации.</param>
    /// <param name="scaleFactor">Множитель масштаба (например, 1.2f для увеличения на 20%).</param>
    /// <param name="duration">Длительность одной фазы анимации (в секундах).</param>
    public IEnumerator Animate(Transform target, float scaleFactor = 1.2f, float duration = 0.3f)
    {
        if (target == null || target.GetComponent<RectTransform>() == null)
        {
            Debug.LogError("Целевой объект для анимации не назначен или не имеет RectTransform!");
            yield break;
        }

        RectTransform targetRectTransform = target.GetComponent<RectTransform>();
        Vector3 originalScale = targetRectTransform.localScale;
        Vector3 expandedScale = originalScale * scaleFactor;

        // Увеличиваем масштаб
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            targetRectTransform.localScale = Vector3.Lerp(originalScale, expandedScale, elapsed / duration);
            yield return null;
        }

        // Возвращаем к исходному масштабу
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            targetRectTransform.localScale = Vector3.Lerp(expandedScale, originalScale, elapsed / duration);
            yield return null;
        }
    }
}