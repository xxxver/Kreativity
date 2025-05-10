using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    [Header("Спрайты для инпутов")]
    public Sprite defaultInputSprite;
    public Sprite errorInputSprite;

    private FirebaseAuth auth;
    private FirebaseFirestore db;
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

        if (dependencyTask.Exception != null)
        {
            Debug.LogError("Ошибка при проверке зависимостей Firebase");
            yield break;
        }

        var initializationTask = InitializeFirebaseAsync();
        float timeout = 10f;
        float elapsedTime = 0f;

        while (!initializationTask.IsCompleted && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase не был инициализирован в установленный срок");
            yield break;
        }

        SetUIInteractable(true);
    }

    async Task InitializeFirebaseAsync()
    {
        try
        {
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            db = FirebaseFirestore.DefaultInstance;
            await Task.Delay(500);
            isFirebaseInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Ошибка] Инициализация Firebase: {ex}");
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
                user = auth.CurrentUser;
                await LoadUserData();
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

    private async Task LoadUserData()
    {
        if (user == null)
        {
            Debug.LogError("user == null");
            return;
        }

        Debug.Log($"📡 Запрашиваем данные пользователя с ID: {user.UserId}");

        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            Debug.Log("✅ Документ найден.");

            Dictionary<string, object> userData = snapshot.ToDictionary();

            foreach (var pair in userData)
            {
                Debug.Log($"🔍 {pair.Key} = {pair.Value}");
            }

            if (userData.ContainsKey("Name") && userData.ContainsKey("email") && userData.ContainsKey("balls"))
            {
                string name = userData["Name"]?.ToString();
                string email = userData["email"]?.ToString();
                long balls = userData.ContainsKey("balls") ? (long)userData["balls"] : 0;

                Debug.Log($"🎯 Имя: {name}, Email: {email}, Баллы: {balls}");

                UserData.Instance.SetUserData(name, email, balls);
            }
            else
            {
                Debug.LogError("❌ Некоторые ключи отсутствуют в документе Firestore.");
            }
        }
        else
        {
            Debug.LogError("❌ Документ пользователя не найден в Firestore.");
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
        SetInputSprite(emailInputField, errorInputSprite);
    }

    void ShowPasswordError(string message)
    {
        passwordErrorLabel.text = message;
        passwordErrorLabel.gameObject.SetActive(true);
        passwordErrorIcon.SetActive(true);
        SetInputSprite(passwordInputField, errorInputSprite);
    }

    void HideEmailError()
    {
        emailErrorLabel.gameObject.SetActive(false);
        emailErrorIcon.SetActive(false);
        SetInputSprite(emailInputField, defaultInputSprite);
    }

    void HidePasswordError()
    {
        passwordErrorLabel.gameObject.SetActive(false);
        passwordErrorIcon.SetActive(false);
        SetInputSprite(passwordInputField, defaultInputSprite);
    }

    void HideErrorUI()
    {
        HideEmailError();
        HidePasswordError();
    }

    void SetInputSprite(TMP_InputField inputField, Sprite sprite)
    {
        Image image = inputField.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
        }
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
