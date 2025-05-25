using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

public class SubscriptionManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject payPanel;
    public GameObject cardPanel;
    public GameObject subOrderPanel;

    [Header("UI - Dropdown")]
    public TMP_Dropdown paymentDropdown;
    public TMP_Dropdown subscriptionDropdown;
    public TMP_Text paymentLabel;

    [Header("UI - Поля карты")]
    public TMP_InputField cardNumberInput;
    public TMP_InputField expiryInput;
    public TMP_InputField cvvInput;
    public TMP_InputField fullNameInput;

    [Header("Ошибки")]
    public TMP_Text cardNumberError;
    public TMP_Text expiryError;
    public TMP_Text cvvError;
    public TMP_Text nameError;
    public TMP_Text subscriptionErrorLabel;

    [Header("Кнопки")]
    public Button saveCardButton;
    public Button confirmSubscriptionButton;
    public Button closeCardPanelButton;
    public Button closePayPanelButton;

    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private List<Dictionary<string, object>> userCards = new List<Dictionary<string, object>>();
    private bool suppressCallback = false;
    private bool justActivated = false;
    private DateTime subscriptionEndTime;

    private string secretKey = "f9168c5e-ceb2-4faa-b6bf-329bf39fa1e4";

    void Start()
    {
        InitializeFirebase();
        SetupUI();

        cardNumberInput.onValueChanged.AddListener(MaskCardNumber);
        expiryInput.onValueChanged.AddListener(MaskExpiryDate);
        cvvInput.onValueChanged.AddListener(MaskCVV);
        fullNameInput.onValueChanged.AddListener(MaskFullName);

        subOrderPanel.SetActive(false);
        cardPanel.SetActive(false);

        _ = CheckSubscriptionStatus();
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    void SetupUI()
    {
        subscriptionDropdown.onValueChanged.AddListener(OnSubscriptionPlanChanged);
        paymentDropdown.onValueChanged.AddListener(OnPaymentOptionChanged);
        saveCardButton.onClick.AddListener(OnSaveCardClick);
        confirmSubscriptionButton.onClick.AddListener(OnConfirmSubscriptionClick);
        closeCardPanelButton.onClick.AddListener(() => { cardPanel.SetActive(false); });
        closePayPanelButton.onClick.AddListener(() => { subOrderPanel.SetActive(false); });

        cardNumberError.gameObject.SetActive(false);
        expiryError.gameObject.SetActive(false);
        cvvError.gameObject.SetActive(false);
        nameError.gameObject.SetActive(false);
        subscriptionErrorLabel.gameObject.SetActive(false);

        PopulateSubscriptionPlans();
        _ = CheckActiveCards();

        OnSubscriptionPlanChanged(subscriptionDropdown.value);
    }

    void PopulateSubscriptionPlans()
    {
        subscriptionDropdown.ClearOptions();
        List<string> plans = new List<string> { "1 месяц - 300 руб", "3 месяца - 850 руб", "12 месяцев - 2999 руб" };
        subscriptionDropdown.AddOptions(plans);
    }

    async Task CheckActiveCards()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        DocumentSnapshot snapshot = await db.Collection("users").Document(user.UserId).GetSnapshotAsync();
        userCards.Clear();

        if (snapshot.Exists && snapshot.ContainsField("cards"))
        {
            var cardsRaw = snapshot.GetValue<List<object>>("cards");
            foreach (var obj in cardsRaw)
            {
                if (obj is Dictionary<string, object> cardDict)
                {
                    userCards.Add(cardDict);
                }
            }
        }

        RefreshPaymentDropdown();
    }

    void RefreshPaymentDropdown()
    {
        suppressCallback = true;

        paymentDropdown.ClearOptions();
        List<string> options = new List<string> { "Выберите карту" };

        foreach (var card in userCards)
        {
            if (card.TryGetValue("number", out object numberObj) && numberObj is string number && number.Length >= 4)
            {
                string masked = number.Substring(0, 4) + " **** **** ****";
                options.Add(masked);
            }
        }

        options.Add("Добавить карту");
        paymentDropdown.AddOptions(options);
        paymentDropdown.value = 0;

        suppressCallback = false;
    }

    void OnPaymentOptionChanged(int index)
    {
        if (suppressCallback) return;

        string selected = paymentDropdown.options[index].text;
        if (selected == "Добавить карту")
        {
            cardPanel.SetActive(true);
        }
    }

    void OnSubscriptionPlanChanged(int index)
    {
        switch (index)
        {
            case 0: paymentLabel.text = "К оплате: 300 руб"; break;
            case 1: paymentLabel.text = "К оплате: 850 руб"; break;
            case 2: paymentLabel.text = "К оплате: 2999 руб"; break;
        }
    }

    async void OnSaveCardClick()
    {
        string cardNum = Regex.Replace(cardNumberInput.text, @"\D", "");
        string expiry = expiryInput.text.Trim();
        string cvv = cvvInput.text.Trim();
        string fullName = fullNameInput.text.Trim();

        cardNumberError.gameObject.SetActive(cardNum.Length != 16);
        expiryError.gameObject.SetActive(!Regex.IsMatch(expiry, @"^\d{2}/\d{2}$"));
        cvvError.gameObject.SetActive(cvv.Length != 3);
        nameError.gameObject.SetActive(!Regex.IsMatch(fullName, @"^[A-Za-zА-Яа-яЁё]+\s[A-Za-zА-Яа-яЁё]+$"));

        if (cardNum.Length != 16 || !Regex.IsMatch(expiry, @"^\d{2}/\d{2}$") || cvv.Length != 3 || !Regex.IsMatch(fullName, @"^[A-Za-zА-Яа-яЁё]+\s[A-Za-zА-Яа-яЁё]+$"))
            return;

        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        Dictionary<string, object> newCard = new Dictionary<string, object>
        {
            { "number", cardNum },
            { "expiry", expiry },
            { "cvv", EncryptDecrypt(cvv) },
            { "name", fullName },
            { "balance", 5000 }
        };

        DocumentReference userDoc = db.Collection("users").Document(user.UserId);

        await userDoc.UpdateAsync(new Dictionary<string, object>
        {
            { "cards", FieldValue.ArrayUnion(newCard) }
        });

        cardPanel.SetActive(false);
        await CheckActiveCards();
    }

    async void OnConfirmSubscriptionClick()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        int selectedIndex = paymentDropdown.value;
        if (selectedIndex <= 0 || selectedIndex - 1 >= userCards.Count)
        {
            subscriptionErrorLabel.text = "Выберите карту для оплаты";
            subscriptionErrorLabel.gameObject.SetActive(true);
            return;
        }

        var selectedCard = userCards[selectedIndex - 1];

        int price = 0;
        int duration = 0;
        switch (subscriptionDropdown.value)
        {
            case 0: price = 300; duration = 30; break;
            case 1: price = 850; duration = 90; break;
            case 2: price = 2999; duration = 365; break;
        }

        int balance = selectedCard.ContainsKey("balance") ? System.Convert.ToInt32(selectedCard["balance"]) : 0;

        if (balance < price)
        {
            subscriptionErrorLabel.text = "Недостаточно средств";
            subscriptionErrorLabel.gameObject.SetActive(true);
            return;
        }

        selectedCard["balance"] = balance - price;

        DateTime endTime = DateTime.UtcNow.AddDays(duration);
        subscriptionEndTime = endTime;

        DocumentReference userDoc = db.Collection("users").Document(user.UserId);

        await userDoc.UpdateAsync(new Dictionary<string, object>
        {
            { "cards", userCards },
            { "subscription", true },
            { "subscriptionEndTime", Timestamp.FromDateTime(endTime) }
        });

        subscriptionErrorLabel.text = "Подписка активирована!";
        subscriptionErrorLabel.gameObject.SetActive(true);

        subOrderPanel.SetActive(false);
        justActivated = true;

        InvokeRepeating("CheckSubscriptionExpiry", 0f, 60f);
    }

    void CheckSubscriptionExpiry()
    {
        if (DateTime.UtcNow >= subscriptionEndTime)
        {
            CancelInvoke("CheckSubscriptionExpiry");
            _ = DeactivateSubscription();
        }
    }

    async Task DeactivateSubscription()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        DocumentReference userDoc = db.Collection("users").Document(user.UserId);

        await userDoc.UpdateAsync(new Dictionary<string, object>
        {
            { "subscription", false },
            { "subscriptionEndTime", null }
        });

        Debug.Log("📴 Подписка деактивирована.");
    }

    async Task CheckSubscriptionStatus()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        DocumentSnapshot snapshot = await db.Collection("users").Document(user.UserId).GetSnapshotAsync();

        bool isSubscribed = false;
        if (snapshot.Exists && snapshot.ContainsField("subscription"))
            isSubscribed = snapshot.GetValue<bool>("subscription");

        if (isSubscribed && snapshot.ContainsField("subscriptionEndTime"))
        {
            Timestamp ts = snapshot.GetValue<Timestamp>("subscriptionEndTime");
            subscriptionEndTime = ts.ToDateTime();
            InvokeRepeating("CheckSubscriptionExpiry", 0f, 60f);
        }

        if (justActivated)
        {
            justActivated = false;
        }
    }

    public void MaskFullName(string input)
    {
        string cleaned = Regex.Replace(input, @"[^A-Za-zА-Яа-яЁё\s]", "");
        cleaned = cleaned.TrimStart();

        int spaceIndex = cleaned.IndexOf(' ');
        if (spaceIndex != -1)
        {
            string beforeSpace = cleaned.Substring(0, spaceIndex);
            string afterSpace = cleaned.Substring(spaceIndex + 1).Replace(" ", "");
            cleaned = beforeSpace + " " + afterSpace;
        }

        if (input != cleaned)
        {
            int newCaretPos = cleaned.Length;
            fullNameInput.SetTextWithoutNotify(cleaned);
            fullNameInput.caretPosition = newCaretPos;
            fullNameInput.stringPosition = newCaretPos;
        }
    }

    public void MaskCVV(string input)
    {
        string digits = Regex.Replace(input, @"\D", "");
        if (digits.Length > 3)
            digits = digits.Substring(0, 3);

        if (cvvInput.text != digits)
        {
            int caretPos = digits.Length;
            cvvInput.SetTextWithoutNotify(digits);
            cvvInput.caretPosition = caretPos;
            cvvInput.stringPosition = caretPos;
        }
    }

    public void MaskExpiryDate(string input)
    {
        string digits = Regex.Replace(input, @"\D", "");
        string result = "";

        if (digits.Length <= 2)
            result = digits;
        else if (digits.Length <= 4)
            result = digits.Substring(0, 2) + "/" + digits.Substring(2);
        else
            result = digits.Substring(0, 2) + "/" + digits.Substring(2, 2);

        if (expiryInput.text != result)
        {
            int caretPos = result.Length;
            expiryInput.SetTextWithoutNotify(result);
            expiryInput.caretPosition = caretPos;
            expiryInput.stringPosition = caretPos;
        }
    }

    public void MaskCardNumber(string input)
    {
        string digits = Regex.Replace(input, @"\D", "");
        string result = "";

        for (int i = 0; i < digits.Length && i < 16; i++)
        {
            if (i > 0 && i % 4 == 0)
                result += " ";
            result += digits[i];
        }

        if (cardNumberInput.text != result)
        {
            int caretPos = result.Length;
            cardNumberInput.SetTextWithoutNotify(result);
            cardNumberInput.caretPosition = caretPos;
            cardNumberInput.stringPosition = caretPos;
        }
    }

    private string EncryptDecrypt(string text)
    {
        char[] key = secretKey.ToCharArray();
        char[] input = text.ToCharArray();
        for (int i = 0; i < input.Length; i++)
            input[i] ^= key[i % key.Length];
        return new string(input);
    }
}
