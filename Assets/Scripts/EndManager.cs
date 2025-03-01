using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;

public class EndManager : MonoBehaviour
{
    public TextMeshProUGUI g1;
    public TextMeshProUGUI g2;
    public TextMeshProUGUI g3;
    public TextMeshProUGUI title;

    public enum Player { Player1, Player2 } // Private enum field for players
    private static List<Player> Winners = new List<Player>(); // Stores the selected player

    private void Start()
    {
        g1.text = (Winners[0] == Player.Player1) ? "Player 1" : "Player 2";
        g2.text = (Winners[1] == Player.Player1) ? "Player 1" : "Player 2";
        g3.text = (Winners[2] == Player.Player1) ? "Player 1" : "Player 2";

        int player1Count = Winners.Count(p => p == Player.Player1);
        title.text = player1Count == 2 ? "Player 1 Wins!" : "Player 2 Wins!";

    }



    public static void ClearWinners()
    {
        Winners.Clear();
    }
    public static void AddWinner(Player winner)
    {
        Winners.Add(winner);
    }
}
