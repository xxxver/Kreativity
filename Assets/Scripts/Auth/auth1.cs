using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData Instance;

    public string userName;
    public string email;
    public long balls;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // сохраняем при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUserData(string name, string email, long balls)
    {
        this.userName = name;
        this.email = email;
        this.balls = balls;
    }
}
