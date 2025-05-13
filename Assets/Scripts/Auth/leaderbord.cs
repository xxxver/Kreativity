// LeaderUI.cs
using TMPro;
using UnityEngine;

public class LeaderUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text ballsText;

    public void SetData(string name, string ballsTextFormatted)
    {
        nameText.text = name;
        ballsText.text = ballsTextFormatted;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
