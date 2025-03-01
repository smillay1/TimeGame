using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        EndManager.ClearWinners();
        SceneManager.LoadScene(1);
    }
}
