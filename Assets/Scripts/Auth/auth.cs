using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;

    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool isFirebaseInitialized = false;
    private Coroutine initCoroutine;

    void Start()
    {
        loginButton.onClick.AddListener(LoginButtonClick);
        SetUIInteractable(false);
        initCoroutine = StartCoroutine(InitializeFirebaseWithTimeout());
    }

    IEnumerator InitializeFirebaseWithTimeout()
    {
        Debug.Log("[Инициализация] Запуск процесса инициализации Firebase");
        
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        if (dependencyTask.Exception != null)
        {
            Debug.LogError($"[Ошибка] Ошибка зависимостей: {dependencyTask.Exception}");
            yield break;
        }

        Debug.Log("[Инициализация] Зависимости проверены");
        
        var initializationTask = InitializeFirebaseAsync();
        float timeout = 10f;
        float elapsedTime = 0f;

        while (!initializationTask.IsCompleted && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            if (Mathf.FloorToInt(elapsedTime) % 2 == 0)
            {
                Debug.Log($"[Инициализация] Ожидание... {Mathf.FloorToInt(timeout - elapsedTime)} сек.");
            }
            yield return null;
        }

        if (!isFirebaseInitialized)
        {
            Debug.LogError("[Ошибка] Инициализация не завершена");
            yield break;
        }

        SetUIInteractable(true);
        Debug.Log("[Инициализация] Готово! Интерфейс активирован");
    }

    async Task InitializeFirebaseAsync()
    {
        try
        {
            Debug.Log("[Инициализация] Создание экземпляра FirebaseAuth");
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            await Task.Delay(500); // Искусственная задержка для теста
            
            Debug.Log("[Инициализация] FirebaseAuth создан");
            isFirebaseInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Ошибка] Инициализация: {ex}");
        }
    }

    public async void LoginButtonClick()
    {
        Debug.Log("[Кнопка] Нажатие кнопки входа зафиксировано");
        if (!ValidateInputs()) return;
        
        SetUIInteractable(false);
        Debug.Log("[Авторизация] Начало процесса входа");

        try
        {
            string email = emailInputField.text;
            string password = passwordInputField.text;
            
            Debug.Log($"[Авторизация] Попытка входа для: {email}");
            
            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            await loginTask;

            if (loginTask.IsCompletedSuccessfully)
            {
                Debug.Log($"[Успех] Успешный вход! Пользователь: {auth.CurrentUser.Email}");
                SceneManager.LoadScene("Home"); // Переход на главную сцену
            }
            else
            {
                Debug.LogError($"[Ошибка] Ошибка входа: {loginTask.Exception}");
            }
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"[Ошибка] Firebase Exception: {ex.Message}");
        }
        finally
        {
            SetUIInteractable(true);
            Debug.Log("[Авторизация] Процесс входа завершен");
        }
    }

    bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(emailInputField.text))
        {
            Debug.LogError("[Валидация] Поле email пустое");
            return false;
        }

        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            Debug.LogError("[Валидация] Поле пароля пустое");
            return false;
        }

        if (!IsValidEmail(emailInputField.text))
        {
            Debug.LogError("[Валидация] Некорректный email");
            return false;
        }

        Debug.Log("[Валидация] Данные валидны");
        return true;
    }

    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    void SetUIInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passwordInputField.interactable = interactable;
        loginButton.interactable = interactable;
        
        Debug.Log($"[Интерфейс] Состояние интерфейса: {(interactable ? "активен" : "заблокирован")}");
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
        
        if (initCoroutine != null)
        {
            StopCoroutine(initCoroutine);
        }
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            user = auth.CurrentUser;
            if (user != null)
            {
                Debug.Log($"[Авторизация] Состояние изменено: Пользователь {user.Email} вошел");
            }
            else
            {
                Debug.Log("[Авторизация] Состояние изменено: Пользователь вышел");
            }
        }
    }
}
