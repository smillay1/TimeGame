//Script handling main starting menu functionality

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public enum Player { Player1, Player2 } // Private enum field for players
    private static List<Player> Winners = new List<Player>(); // Stores the selected player
    private static List<string> SceneOrder = new List<string> { "Jack", "Hope", "Camille" };
    private static int CurrScene = 0;

    public void StartGame()
    {
        Winners.Clear();
        CurrScene = 0;
        SceneManager.LoadScene(SceneOrder[CurrScene]);
    }

    public static void NextScene(Player winner)
    {
        Winners.Add(winner);
        CurrScene = (CurrScene + 1) % SceneOrder.Count;
        SceneManager.LoadScene(SceneOrder[CurrScene]);
    }
}
