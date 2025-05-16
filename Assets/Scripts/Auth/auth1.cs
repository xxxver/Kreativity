using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : MonoBehaviour
{
    public static UserData Instance;

    public string Name;
    public string email;
    public long balls;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUserData(string name, string email, long balls)
    {
        this.Name = name;
        this.email = email;
        this.balls = balls;
    }
    public void ClearUserDataAndReloadScene(string sceneName)
    {
        Name = string.Empty;
        email = string.Empty;
        balls = 0;

        Instance = null;
        Destroy(gameObject); // ← Удаляем объект UserData

        SceneManager.LoadScene(sceneName);
    }

}
