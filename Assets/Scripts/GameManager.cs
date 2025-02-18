using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI targetTimeText;
    public GameObject horse1Prefab;
    public GameObject horse2Prefab;
    public float moveDistance = 1.5f;
    public float finishLineX = 8f;

    private GameObject horse1;
    private GameObject horse2;
    private float targetTime;
    private float startTime;
    private bool roundActive = false;
    private float player1Time = 0f;
    private float player2Time = 0f;

    void Start()
    {
        SpawnHorses();
        StartNewRound();
    }

    void Update()
    {
        if (roundActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && player1Time == 0)
            {
                player1Time = Time.time - startTime;
                Debug.Log("Player 1 guessed: " + player1Time);
            }

            if (Input.GetKeyDown(KeyCode.RightShift) && player2Time == 0)
            {
                player2Time = Time.time - startTime;
                Debug.Log("Player 2 guessed: " + player2Time);
            }

            if (player1Time > 0 && player2Time > 0)
            {
                DetermineRoundWinner();
            }
            
        }
    }

    void SpawnHorses()
    {
        Debug.Log("Spawning horses...");

        if (horse1Prefab == null || horse2Prefab == null)
        {
            Debug.LogError("Horse prefabs are not assigned!");
            return;
        }

        float leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;

        horse1 = Instantiate(horse1Prefab, new Vector3(leftEdge, -2f, 0), Quaternion.identity);
        horse2 = Instantiate(horse2Prefab, new Vector3(leftEdge, -3.1f, 0), Quaternion.identity);

        Debug.Log("Horse 1 spawned at: " + horse1.transform.position);
        Debug.Log("Horse 2 spawned at: " + horse2.transform.position);
    }

    void StartNewRound()
{
    targetTime = Random.Range(1f, 7f);

    if (targetTimeText == null)
    {
        Debug.LogError("Target Time Text is not assigned in GameManager!");
        return;
    }

    // Clear and update the text
    targetTimeText.text = ""; // Clear the previous text
    targetTimeText.text = "Target Time: " + targetTime.ToString("F2") + "s";

    // Force UI to refresh
    targetTimeText.ForceMeshUpdate();
    Canvas.ForceUpdateCanvases();
    targetTimeText.SetAllDirty();

    Debug.Log("New target time displayed on UI: " + targetTime);
    
    startTime = Time.time;
    player1Time = 0f;
    player2Time = 0f;
    roundActive = true;
}





    void DetermineRoundWinner()
    {
        roundActive = false;

        float player1Difference = Mathf.Abs(targetTime - player1Time);
        float player2Difference = Mathf.Abs(targetTime - player2Time);

        if (player1Difference < player2Difference)
        {
            StartCoroutine(MoveHorseSmoothly(horse1, new Vector3(moveDistance, 0, 0), 1f));
            Debug.Log("Player 1 wins this round! Horse moved to: " + horse1.transform.position);
        }
        else if (player2Difference < player1Difference)
        {
            StartCoroutine(MoveHorseSmoothly(horse2, new Vector3(moveDistance, 0, 0), 1f));
            Debug.Log("Player 2 wins this round! Horse moved to: " + horse2.transform.position);
        }
        else
        {
            Debug.Log("Tie round! No movement.");
        }

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (horse1.transform.position.x >= finishLineX)
        {
            targetTimeText.text = "Player 1 Wins!";
            StartCoroutine(RestartGame());
        }
        else if (horse2.transform.position.x >= finishLineX)
        {
            targetTimeText.text = "Player 2 Wins!";
            StartCoroutine(RestartGame());
        }
        else
        {
            StartCoroutine(WaitBeforeNextRound());
        }
    }

    IEnumerator WaitBeforeNextRound()
{
    Debug.Log("Waiting before next round...");
    yield return new WaitForSeconds(2f); 

    Debug.Log("Starting new round...");
    StartNewRound();
}


    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3f);

        float leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;
        horse1.transform.position = new Vector3(leftEdge, -2f, 0);
        horse2.transform.position = new Vector3(leftEdge, -3.1f, 0);

        StartNewRound();
    }

    IEnumerator MoveHorseSmoothly(GameObject horse, Vector3 moveOffset, float duration)
    {
        Vector3 startPosition = horse.transform.position;
        Vector3 targetPosition = startPosition + moveOffset;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            horse.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        horse.transform.position = targetPosition; // Ensure the final position is exact
    }

}
