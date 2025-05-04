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
    [Header("UI поля")]
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;

    [Header("Тексты ошибок")]
    public TMP_Text emailErrorLabel;
    public TMP_Text passwordErrorLabel;

    [Header("Иконки ошибок")]
    public GameObject emailErrorIcon;
    public GameObject passwordErrorIcon;

    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool isFirebaseInitialized = false;
    private Coroutine initCoroutine;

    void Start()
    {
        loginButton.onClick.AddListener(LoginButtonClick);
        SetUIInteractable(false);
        HideErrorUI();
        initCoroutine = StartCoroutine(InitializeFirebaseWithTimeout());
    }

    IEnumerator InitializeFirebaseWithTimeout()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        if (dependencyTask.Exception != null) yield break;

        var initializationTask = InitializeFirebaseAsync();
        float timeout = 10f;
        float elapsedTime = 0f;

        while (!initializationTask.IsCompleted && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isFirebaseInitialized) yield break;

        SetUIInteractable(true);
    }

    async Task InitializeFirebaseAsync()
    {
        try
        {
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            await Task.Delay(500);
            isFirebaseInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Ошибка] Инициализация: {ex}");
        }
    }

    public async void LoginButtonClick()
    {
        if (!ValidateInputs()) return;

        SetUIInteractable(false);

        try
        {
            string email = emailInputField.text;
            string password = passwordInputField.text;

            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            await loginTask;

            if (loginTask.IsCompletedSuccessfully)
            {
                SceneManager.LoadScene("Home");
            }
            else
            {
                HandleFirebaseError(loginTask.Exception);
            }
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"[Ошибка Firebase] {ex.Message}");
            HandleFirebaseError(ex);
        }
        finally
        {
            SetUIInteractable(true);
        }
    }

    void HandleFirebaseError(System.Exception exception)
    {
        FirebaseException firebaseEx = exception as FirebaseException;
        if (firebaseEx != null)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                    ShowEmailError("Неверный email");
                    break;
                case AuthError.WrongPassword:
                    ShowPasswordError("Неверный пароль");
                    break;
                case AuthError.UserNotFound:
                    ShowEmailError("Пользователь не найден");
                    break;
                default:
                    ShowEmailError("Неверный email");
                    ShowPasswordError("Неверный пароль");
                    break;
            }
        }
    }

    bool ValidateInputs()
    {
        bool isValid = true;

        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;

        // Email проверка
        if (string.IsNullOrWhiteSpace(email))
        {
            ShowEmailError("Введите email");
            isValid = false;
        }
        else if (!IsValidEmail(email))
        {
            ShowEmailError("Неверный формат email");
            isValid = false;
        }
        else
        {
            HideEmailError();
        }

        // Password проверка (только на пустоту)
        if (string.IsNullOrWhiteSpace(password))
        {
            ShowPasswordError("Введите пароль");
            isValid = false;
        }
        else
        {
            HidePasswordError();
        }

        return isValid;
    }

    void ShowEmailError(string message)
    {
        emailErrorLabel.text = message;
        emailErrorLabel.gameObject.SetActive(true);
        emailErrorIcon.SetActive(true);
    }

    void ShowPasswordError(string message)
    {
        passwordErrorLabel.text = message;
        passwordErrorLabel.gameObject.SetActive(true);
        passwordErrorIcon.SetActive(true);
    }

    void HideEmailError()
    {
        emailErrorLabel.gameObject.SetActive(false);
        emailErrorIcon.SetActive(false);
    }

    void HidePasswordError()
    {
        passwordErrorLabel.gameObject.SetActive(false);
        passwordErrorIcon.SetActive(false);
    }

    void HideErrorUI()
    {
        HideEmailError();
        HidePasswordError();
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
                Debug.Log($"[Firebase] Пользователь вошёл: {user.Email}");
            }
            else
            {
                Debug.Log("[Firebase] Пользователь вышел");
            }
        }
    }
}