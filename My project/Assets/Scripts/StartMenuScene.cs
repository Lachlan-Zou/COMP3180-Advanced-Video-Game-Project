using UnityEngine;
using UnityEngine.SceneManagement; // Required for managing scenes

public class NewMonoBehaviourScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Maze");

    }
}
