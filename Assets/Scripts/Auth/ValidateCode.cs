using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VerificationCodePanel : MonoBehaviour
{
    public GameObject panelAccept;
    public TMP_InputField codeInputField;
    public TMP_Text errorLabel;
    public Button submitButton;
    public Button resendButton;
    public TMP_Text timerText;

    public FirebaseRegistrationManager registrationManager; // Добавьте это поле и установите его в инспекторе Unity

    private string expectedCode;
    private float resendCooldown = 30f;
    private float timeLeft;
    private bool waiting = false;

    void Start()
    {
        panelAccept.SetActive(false);
        resendButton.interactable = false;
        submitButton.onClick.AddListener(OnSubmit);
        resendButton.onClick.AddListener(OnResend);
    }

    public void Show(string code)
    {
        expectedCode = code;
        panelAccept.SetActive(true);
        codeInputField.text = "";
        errorLabel.gameObject.SetActive(false);
        StartCoroutine(StartResendCooldown());
    }

    void OnSubmit()
    {
        if (codeInputField == null || errorLabel == null)
        {
            Debug.LogError("Один из компонентов не установлен!");
            return;
        }

        if (codeInputField.text.Trim() != expectedCode)
        {
            errorLabel.text = "Неверный код";
            errorLabel.gameObject.SetActive(true);
        }
        else
        {
            errorLabel.gameObject.SetActive(false);

            if (registrationManager != null)
            {
                registrationManager.CompleteRegistration();
            }
            else
            {
                Debug.LogError("FirebaseRegistrationManager не установлен!");
            }
        }
    }

    void OnResend()
    {
        if (!waiting)
        {
            if (registrationManager != null)
            {
                registrationManager.ResendVerificationCode();
                StartCoroutine(StartResendCooldown());
            }
            else
            {
                Debug.LogError("FirebaseRegistrationManager не установлен!");
            }
        }
    }

    IEnumerator StartResendCooldown()
    {
        waiting = true;
        resendButton.interactable = false;
        timeLeft = resendCooldown;

        while (timeLeft > 0)
        {
            timerText.text = "Отправить повторно через: " + Mathf.CeilToInt(timeLeft) + "с";
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        timerText.text = "";
        resendButton.interactable = true;
        waiting = false;
    }
}
