using UnityEngine;
using UnityEngine.EventSystems;

public class PaletteDrag4: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup; 

    [HideInInspector] public Transform originalParent;
    [HideInInspector] public Vector2 originalPosition;
    [HideInInspector] private Vector3 originalScale;

    public bool isCorrectPalette;
    private bool wasAttachedToTab = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        originalScale = transform.localScale; // Сохраняем масштаб
    }

    public Vector3 OriginalScale => originalScale; // Геттер для оригинального масштаба

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform); // Свободное перетаскивание
        canvasGroup.blocksRaycasts = false;
        wasAttachedToTab = (transform.parent != originalParent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 globalMousePos);

        rectTransform.position = globalMousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!TabSlot4.lastDroppedOnThisFrame)
        {
            // Отвязать и вернуть на старт
            ReturnToStart();
            TabChecker24.Instance.ClearPalette();
        }

        TabSlot4.lastDroppedOnThisFrame = false;
    }

    public void ReturnToStart()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        transform.localScale = originalScale; // Возвращаем исходный размер
    }
}