using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Необходимо для работы с UI элементами

public class LoadSceneOnButtonClick : MonoBehaviour
{
    // Метод для загрузки сцены
    public void LoadScene()
    {
        SceneManager.LoadScene("Profile");
    }
}
