using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "LoadoutScene"; // <-- Change to your scene name

    public void StartGame()
    {
        // Optional: You can validate data here (skin chosen, hair chosen, etc.)

        SceneManager.LoadScene(gameSceneName);
    }
}
