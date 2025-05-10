using TMPro;
using UnityEngine;

public class HomeUIController : MonoBehaviour
{
    public TMP_Text nameLabel;
    public TMP_Text ballsLabel;

    void Start()
    {
        if (UserData.Instance != null)
        {
            nameLabel.text = UserData.Instance.Name;
            ballsLabel.text = $"{UserData.Instance.balls.ToString()} баллов";
        }
        else
        {
            Debug.LogError("UserData.Instance is null!");
        }
    }
}
