using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragAndDrop1 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform zone;
    public Image[] imagesToShow;
    public Canvas canvas;
    public float dragSpeed = 10f; // скорость следования за курсором
    private Vector3 startPosition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private static int draggedItemsCount = 0; // Счетчик перетащенных объектов
    public GameObject winPanel; // Панель для отображения, когда все объекты перетащены
    public Button confirmButton; // Кнопка для перехода на сцену "Level1-1"
    public Button closeButton; // Кнопка для перехода на сцену "Home"

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        foreach (Image img in imagesToShow)
        {
            img.gameObject.SetActive(false); // Изначально изображения скрыты
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false); // Панель win скрыта в начале
        }

        // Привязываем действия к кнопкам
        confirmButton.onClick.AddListener(GoToLevel1);
        closeButton.onClick.AddListener(GoToHome);

        // Подписываемся на событие выгрузки сцены
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Когда сцена выгружается, сбрасываем счетчик
        ResetCounter();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out worldPoint);
        Vector3 targetPosition = new Vector3(worldPoint.x, worldPoint.y, rectTransform.position.z);
        rectTransform.position = Vector3.Lerp(rectTransform.position, targetPosition, Time.deltaTime * dragSpeed);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (RectTransformUtility.RectangleContainsScreenPoint(zone, eventData.position, eventData.pressEventCamera))
        {
            // Скрываем иконку и зону, но не деактивируем объект полностью
            gameObject.GetComponent<Image>().enabled = false; // Скрыть изображение
            zone.gameObject.SetActive(false); // Скрыть зону

            // Перед запуском корутины активируем изображения
            foreach (Image img in imagesToShow)
            {
                img.gameObject.SetActive(true);
            }

            // Запускаем корутину для показа изображения и анимации
            StartCoroutine(ShowAndAnimateImage());

            // Увеличиваем счетчик перетащенных объектов
            draggedItemsCount++;
            Debug.Log($"Перетащено объектов: {draggedItemsCount}");

            // Проверяем, если все объекты были перетащены
            CheckForWin();
        }
        else
        {
            StartCoroutine(ReturnToStart());
        }
    }

    private IEnumerator ShowAndAnimateImage()
    {
        // Проверяем, что изображение существует и активно
        if (imagesToShow[0] != null && imagesToShow[0].gameObject.activeInHierarchy)
        {
            // Берем первое изображение для анимации
            RectTransform imgRectTransform = imagesToShow[0].rectTransform; 
            Vector3 initialScale = imgRectTransform.localScale;
            Vector3 targetScale = new Vector3(1.1f, 1.1f, 1); // Раздувание до 110%
            Vector3 initialPosition = imgRectTransform.localPosition;
            Vector3 targetPosition = initialPosition + new Vector3(10, 10, 0); // Легкое смещение по X и Y

            float elapsedTime = 0f;
            float duration = 0.5f; // Сделать анимацию за 0.5 секунды

            // Анимация увеличения и смещения
            while (elapsedTime < duration)
            {
                imgRectTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                imgRectTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            imgRectTransform.localScale = targetScale;
            imgRectTransform.localPosition = targetPosition;

            // Анимация возвращения к исходному положению без задержки
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                imgRectTransform.localScale = Vector3.Lerp(targetScale, initialScale, elapsedTime / duration);
                imgRectTransform.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            imgRectTransform.localScale = initialScale;
            imgRectTransform.localPosition = initialPosition;

            // Изображение не исчезает, оставляем его активным
        }
        else
        {
            Debug.LogWarning("Изображение было уничтожено или деактивировано.");
        }
    }

    private IEnumerator ReturnToStart()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        Vector2 initialPos = rectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(initialPos, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = startPosition;
    }

    private void CheckForWin()
    {
        // Проверяем, если все 5 объектов были перетащены
        if (draggedItemsCount >= 5 && winPanel != null)
        {
            // Запускаем корутину для задержки перед отображением панели win
            StartCoroutine(ShowWinPanelWithDelay());
        }
    }

    private IEnumerator ShowWinPanelWithDelay()
    {
        // Ждем 3 секунды перед показом панели
        yield return new WaitForSeconds(3f);

        // После задержки показываем панель win
        winPanel.SetActive(true);
        Debug.Log("Все объекты перетащены! Панель Win показывается.");
    }

    private void GoToLevel1()
    {
        // Переход на сцену "Level1-1"
        SceneManager.LoadScene("Home");
    }

    private void GoToHome()
    {
        // Переход на сцену "Home"
        SceneManager.LoadScene("Home");
    }

    private void ResetCounter()
    {
        draggedItemsCount = 0;
        Debug.Log("Счетчик сброшен при выходе из сцены.");
    }
}
