using UnityEngine;

public class FinishLine2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            Debug.Log("Player 1 Wins!");
            GameManager.Instance.StartCoroutine(GameManager.Instance.RestartGame(MainMenuManager.Player.Player1));
        }
        else if (other.CompareTag("Player2"))
        {
            Debug.Log("Player 2 Wins!");
            GameManager.Instance.StartCoroutine(GameManager.Instance.RestartGame(MainMenuManager.Player.Player2));
        }
    }
}
