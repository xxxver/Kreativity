using UnityEngine;
using System.Collections.Generic;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject draggedIcon; // Перетаскиваемая иконка

    public void SetInitialPosition(GameObject icon, Vector3 position, Transform parent)
    {
        IconData data = new IconData
        {
            Icon = icon,
            InitialPosition = position,
            InitialParent = parent
        };
        icons.Add(data);
    }

    public void ResetIconPosition(GameObject icon)
    {
        IconData data = icons.Find(x => x.Icon == icon);
        if (data != null) // Now this comparison works because IconData is a class
        {
            data.Icon.transform.SetParent(data.InitialParent, false);
            data.Icon.transform.localPosition = data.InitialPosition;
        }
        else
        {
            Debug.LogWarning("Icon not found in the list.");
        }
    }

    private List<IconData> icons = new List<IconData>();

    [System.Serializable]
    public class IconData // Changed from struct to class
    {
        public GameObject Icon;
        public Vector3 InitialPosition;
        public Transform InitialParent;
    }
}