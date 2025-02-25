using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Sam");
    }

    public void StartFrog()
    {
        SceneManager.LoadScene("Camille");
    }
}
