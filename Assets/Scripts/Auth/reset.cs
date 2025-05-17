using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using System.Collections;

public class ForgotPasswordManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField emailInputField;
    public TMP_Text statusLabel;

    public GameObject forgotPasswordPanel;
    public GameObject loginPanel;

    public Button sendButton;
    public Button backButton;

    [Header("Скрыть после отправки")]
    public GameObject[] elementsToHideOnSend;
    public Image successImage;

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        sendButton.onClick.AddListener(OnSendResetEmail);
        backButton.onClick.AddListener(OnBackToLogin);

        ClearStatus();

        if (successImage != null)
            successImage.gameObject.SetActive(false);
    }

    public void OnSendResetEmail()
    {
        ClearStatus();
        string email = emailInputField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            statusLabel.text = "Введите email.";
            return;
        }

        StartCoroutine(SendResetEmailCoroutine(email));
    }

    private IEnumerator SendResetEmailCoroutine(string email)
    {
        var task = auth.SendPasswordResetEmailAsync(email);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError("Ошибка сброса пароля: " + task.Exception?.Message);
            statusLabel.text = "Ошибка отправки письма.";
        }
        else
        {
            Debug.Log("✅ Письмо сброса пароля отправлено на: " + email);
            statusLabel.text = "Письмо отправлено! Проверьте почту.";

            // Скрываем элементы
            foreach (GameObject obj in elementsToHideOnSend)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            // Показываем картинку
            if (successImage != null)
                successImage.gameObject.SetActive(true);
        }
    }

    public void OnBackToLogin()
    {
        forgotPasswordPanel.SetActive(false);
        loginPanel.SetActive(true);
        ClearStatus();
        ClearEmailInput();
    }

    private void ClearStatus()
    {
        statusLabel.text = "";

        // Восстановление скрытых элементов
        foreach (GameObject obj in elementsToHideOnSend)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (successImage != null)
            successImage.gameObject.SetActive(false);
    }

    private void ClearEmailInput()
    {
        if (emailInputField != null)
            emailInputField.text = "";
    }
}
