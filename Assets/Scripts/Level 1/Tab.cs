using UnityEngine;
using UnityEngine.EventSystems;

public class TabButtonController : MonoBehaviour, IPointerClickHandler
{
    public GameObject descriptor; // Ссылка на объект Descriptor

    void Start()
    {
        // Убедитесь, что Descriptor невидим по умолчанию
        if (descriptor != null)
        {
            descriptor.SetActive(false);
            Debug.Log("Descriptor set to inactive in Start.");
        }
        else
        {
            Debug.LogError("Descriptor is null in Start.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Проверяем, что descriptor существует и не был удален
        if (descriptor != null && !descriptor.activeSelf)
        {
            descriptor.SetActive(true);
            Debug.Log("Descriptor activated on pointer click.");

            // Задержка в 3 секунды
            Invoke("HideDescriptor", 3f);
        }
        else
        {
            Debug.LogWarning("Descriptor is null or already active.");
        }
    }

    void HideDescriptor()
    {
        // Проверяем, что descriptor существует и активен
        if (descriptor != null && descriptor.activeSelf)
        {
            descriptor.SetActive(false);
            Debug.Log("Descriptor deactivated after delay.");
        }
        else
        {
            Debug.LogWarning("Descriptor is null or not active.");
        }
    }
}