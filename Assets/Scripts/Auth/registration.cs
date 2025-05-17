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
using System.IO;
using System.Linq;

public class FirebaseRegistrationManager : MonoBehaviour
{
    [Header("UI поля")]
    public TMP_InputField nameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField confirmPasswordInputField;
    public Button registerButton;

    [Header("PanelAccept")]
    public VerificationCodePanel verificationPanel;

    [Header("Тексты ошибок")]
    public TMP_Text nameErrorLabel;
    public TMP_Text emailErrorLabel;
    public TMP_Text passwordErrorLabel;
    public TMP_Text confirmPasswordErrorLabel;

    [Header("Иконки ошибок")]
    public GameObject nameErrorIcon;
    public GameObject emailErrorIcon;
    public GameObject passwordErrorIcon;
    public GameObject confirmPasswordErrorIcon;

    [Header("Спрайты для инпутов")]
    public Sprite defaultInputSprite;
    public Sprite errorInputSprite;

    public EmailSender emailSender; // Установите это в инспекторе Unity

    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private FirebaseUser user;
    private bool isFirebaseInitialized = false;
    private Coroutine initCoroutine;
    private HashSet<string> bannedWords = new HashSet<string>();
    private string currentEmail;
    private string currentPassword;
    private string currentName;
    private string verificationCode;

    void Start()
    {
        registerButton.onClick.AddListener(RegisterButtonClick);
        SetUIInteractable(false);
        HideErrorUI();
        LoadBannedWords();
        initCoroutine = StartCoroutine(InitializeFirebaseWithTimeout());
    }

    void LoadBannedWords()
    {
        string path = Path.Combine(Application.dataPath, "Scripts/Auth/ban.txt");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            bannedWords = new HashSet<string>(lines.Select(w => w.Trim().ToLower()));
        }
        else
        {
            Debug.LogWarning("❗ Файл ban.txt не найден по пути: " + path);
        }
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
            db = FirebaseFirestore.DefaultInstance;
            await Task.Delay(500);
            isFirebaseInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Ошибка] Инициализация Firebase: {ex}");
        }
    }

    public async void RegisterButtonClick()
{
    if (!ValidateInputs()) return;

    SetUIInteractable(false);

    currentName = nameInputField.text.Trim();
    currentEmail = emailInputField.text.Trim();
    currentPassword = passwordInputField.text;

    if (bannedWords.Contains(currentName.ToLower()))
    {
        ShowError(nameErrorLabel, nameErrorIcon, nameInputField, "Имя недопустимо");
        SetUIInteractable(true);
        return;
    }

    bool isUniqueName = await IsNameUnique(currentName);
    if (!isUniqueName)
    {
        ShowError(nameErrorLabel, nameErrorIcon, nameInputField, "Имя уже используется");
        SetUIInteractable(true);
        return;
    }

    bool isUniqueEmail = await IsEmailUnique(currentEmail);
    if (!isUniqueEmail)
    {
        ShowError(emailErrorLabel, emailErrorIcon, emailInputField, "Email уже используется");
        SetUIInteractable(true);
        return;
    }

    verificationCode = Random.Range(100000, 999999).ToString();
    StartCoroutine(SendVerificationCode(currentEmail, verificationCode));
    verificationPanel.Show(verificationCode);
}
private async Task<bool> IsEmailUnique(string email)
{
    QuerySnapshot snapshot = await db.Collection("users")
        .WhereEqualTo("email", email)
        .GetSnapshotAsync();
    return snapshot.Count == 0;
}


    public void ResendVerificationCode()
    {
        if (!string.IsNullOrEmpty(currentEmail) && !string.IsNullOrEmpty(verificationCode))
        {
            StartCoroutine(SendVerificationCode(currentEmail, verificationCode));
        }
    }

    public async void CompleteRegistration()
    {
        try
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(currentEmail, currentPassword);
            await registerTask;

            if (registerTask.IsCompletedSuccessfully)
            {
                user = auth.CurrentUser;
                await SaveUserData();
                UserData.Instance.SetUserData(currentName, currentEmail, 0);
                SceneManager.LoadScene("Home");
            }
            else
            {
                HandleFirebaseError(registerTask.Exception);
            }
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"[Ошибка Firebase] {ex.Message}");
            HandleFirebaseError(ex);
        }
    }

    IEnumerator SendVerificationCode(string email, string code)
    {
        if (emailSender != null)
        {
            yield return emailSender.SendVerificationEmail(email, code);
        }
        else
        {
            Debug.LogError("EmailSender не установлен!");
        }
    }

    private async Task<bool> IsNameUnique(string nickname)
    {
        QuerySnapshot snapshot = await db.Collection("users")
            .WhereEqualTo("Name", nickname)
            .GetSnapshotAsync();
        return snapshot.Count == 0;
    }

    private async Task SaveUserData()
    {
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"Name", currentName},
            {"email", currentEmail},
            {"Balls", 0}
        };

        await db.Collection("users").Document(auth.CurrentUser.UserId).SetAsync(userData);
    }

    bool ValidateInputs()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            ShowError(nameErrorLabel, nameErrorIcon, nameInputField, "Введите имя");
            isValid = false;
        }
        else
        {
            HideError(nameErrorLabel, nameErrorIcon, nameInputField);
        }

        if (string.IsNullOrWhiteSpace(emailInputField.text) || !IsValidEmail(emailInputField.text))
        {
            ShowError(emailErrorLabel, emailErrorIcon, emailInputField, "Введите корректный email");
            isValid = false;
        }
        else
        {
            HideError(emailErrorLabel, emailErrorIcon, emailInputField);
        }

        if (passwordInputField.text.Length < 6)
        {
            ShowError(passwordErrorLabel, passwordErrorIcon, passwordInputField, "Пароль не менее 6 символов");
            isValid = false;
        }
        else
        {
            HideError(passwordErrorLabel, passwordErrorIcon, passwordInputField);
        }

        if (passwordInputField.text != confirmPasswordInputField.text)
        {
            ShowError(confirmPasswordErrorLabel, confirmPasswordErrorIcon, confirmPasswordInputField, "Пароли не совпадают");
            isValid = false;
        }
        else
        {
            HideError(confirmPasswordErrorLabel, confirmPasswordErrorIcon, confirmPasswordInputField);
        }

        return isValid;
    }

    void ShowError(TMP_Text label, GameObject icon, TMP_InputField field, string message)
    {
        label.text = message;
        label.gameObject.SetActive(true);
        icon.SetActive(true);
        field.GetComponent<Image>().sprite = errorInputSprite;
    }

    void HideError(TMP_Text label, GameObject icon, TMP_InputField field)
    {
        label.gameObject.SetActive(false);
        icon.SetActive(false);
        field.GetComponent<Image>().sprite = defaultInputSprite;
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
        nameInputField.interactable = interactable;
        emailInputField.interactable = interactable;
        passwordInputField.interactable = interactable;
        confirmPasswordInputField.interactable = interactable;
        registerButton.interactable = interactable;
    }

    void HideErrorUI()
    {
        HideError(nameErrorLabel, nameErrorIcon, nameInputField);
        HideError(emailErrorLabel, emailErrorIcon, emailInputField);
        HideError(passwordErrorLabel, passwordErrorIcon, passwordInputField);
        HideError(confirmPasswordErrorLabel, confirmPasswordErrorIcon, confirmPasswordInputField);
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
                    ShowError(emailErrorLabel, emailErrorIcon, emailInputField, "Неверный email");
                    break;
                case AuthError.EmailAlreadyInUse:
                    ShowError(emailErrorLabel, emailErrorIcon, emailInputField, "Email уже используется");
                    break;
                case AuthError.WeakPassword:
                    ShowError(passwordErrorLabel, passwordErrorIcon, passwordInputField, "Слабый пароль");
                    break;
                default:
                    ShowError(emailErrorLabel, emailErrorIcon, emailInputField, "Ошибка регистрации");
                    break;
            }
        }
    }
}
