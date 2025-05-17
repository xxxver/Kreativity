using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject loginPanel;
    public GameObject registrationPanel;
    public GameObject resetPasswordPanel; // Новая панель сброса пароля

    [Header("Кнопки")]
    public Button loginSwitchButton;        // Кнопка "Уже есть аккаунт? Войти"
    public Button registerSwitchButton;     // Кнопка "Нет аккаунта? Зарегистрироваться"
    public Button resetSwitchButton;        // Кнопка "Забыли пароль?"

    void Start()
    {
        // Показываем login, скрываем остальные
        ShowLoginPanel();

        if (loginSwitchButton != null)
            loginSwitchButton.onClick.AddListener(ShowLoginPanel);

        if (registerSwitchButton != null)
            registerSwitchButton.onClick.AddListener(ShowRegistrationPanel);

        if (resetSwitchButton != null)
            resetSwitchButton.onClick.AddListener(ShowResetPasswordPanel);
    }

    void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registrationPanel.SetActive(false);
        resetPasswordPanel.SetActive(false);
    }

    void ShowRegistrationPanel()
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(true);
        resetPasswordPanel.SetActive(false);
    }

    void ShowResetPasswordPanel()
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(false);
        resetPasswordPanel.SetActive(true);
    }
}
