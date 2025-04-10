using UnityEngine;
using UnityEngine.EventSystems;

public class TabSlot3 : MonoBehaviour, IDropHandler
{
    public static bool lastDroppedOnThisFrame = false;

    private PaletteDrag3 currentPalette;

    public void OnDrop(PointerEventData eventData)
    {
        PaletteDrag3 dragged = eventData.pointerDrag?.GetComponent<PaletteDrag3>();

        if (dragged != null)
        {
            // Если уже есть палитра — вернём её обратно
            if (currentPalette != null && currentPalette != dragged)
            {
                currentPalette.ReturnToStart();
            }

            // Привязать новую палитру
            dragged.transform.SetParent(transform);
            dragged.transform.localScale = dragged.OriginalScale * 1.5f; // Увеличиваем в 1.5 раза
            dragged.GetComponent<RectTransform>().anchoredPosition = new Vector2(95f, 0f);

            dragged.GetComponent<RectTransform>().SetAsLastSibling(); // Поверх

            currentPalette = dragged;

            TabChecker23.Instance.SetPalette(dragged);
            lastDroppedOnThisFrame = true;
        }
    }

    public void ClearPalette()
    {
        currentPalette = null;
    }
}