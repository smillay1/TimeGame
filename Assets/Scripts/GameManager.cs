using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI targetTimeText;
    public GameObject horse1Prefab;
    public GameObject horse2Prefab;
    public float moveDistance = 1.5f;
    public float finishLineX = 8f;
    public float timeLow = 1f;
    public float timeHigh = 5f;

    private GameObject horse1;
    private GameObject horse2;
    private TrackEffects track1;
    private TrackEffects track2;
    private float targetTime;
    private float startTime;
    private bool roundActive = false;
    private float player1Time = 0f;
    private float player2Time = 0f;
    public AudioSource clockTickingSound;

    public GameObject speedPowerUpPrefab;
    public GameObject freezePowerUpPrefab;
    private bool powerUpSpawned = false;

    void Start()
    {
        SpawnHorses();
        StartNewRound();
        GetTracks();
    }

    void Update()
    {
        if (roundActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && player1Time == 0)
            {
                player1Time = Time.time - startTime;
                Debug.Log("Player 1 guessed: " + player1Time);
                CheckStopClockSound();
            }

            if (Input.GetKeyDown(KeyCode.RightShift) && player2Time == 0)
            {
                player2Time = Time.time - startTime;
                Debug.Log("Player 2 guessed: " + player2Time);
                CheckStopClockSound();
            }

            if (player1Time > 0 && player2Time > 0)
            {
                DetermineRoundWinner();
            }

        }

        int moveDifference = 0;
        if (horse1 != null && horse2 != null)
        {
            moveDifference = Mathf.Abs(horse1.GetComponent<Player>().MoveCount - horse2.GetComponent<Player>().MoveCount);
        }

        if (moveDifference >= 3 && !powerUpSpawned)
        {
            SpawnPowerUp();
            powerUpSpawned = true; // Only spawn one until collected
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

        horse1 = Instantiate(horse1Prefab, new Vector3(leftEdge, -1.6f, 0), Quaternion.identity);
        horse2 = Instantiate(horse2Prefab, new Vector3(leftEdge, -3.1f, 0), Quaternion.identity);

        Debug.Log("Horse 1 spawned at: " + horse1.transform.position);
        Debug.Log("Horse 2 spawned at: " + horse2.transform.position);
    }

    void StartNewRound()
    {
        roundActive = true;
        Debug.Log("StartNewRound() was called! Target time: " + targetTime);
        targetTime = Random.Range(timeLow, timeHigh);

        if (targetTimeText == null)
        {
            Debug.LogError("Target Time Text is not assigned in GameManager!");
            return;
        }

        // update the text
        targetTimeText.text = "Target Time: " + targetTime.ToString("F2") + "s";

        // Force UI to refresh
        targetTimeText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        targetTimeText.SetAllDirty();

        Debug.Log("New target time displayed on UI: " + targetTime);

        horse1.GetComponent<Player>().ResetMovement();
        horse2.GetComponent<Player>().ResetMovement();
        horse1.GetComponent<Player>().ResetMoveCount();
        horse2.GetComponent<Player>().ResetMoveCount();
        
        startTime = Time.time;
        player1Time = 0f;
        player2Time = 0f;

        roundActive = true;
        PlayClockSound();
    }


    void DetermineRoundWinner()
    {
        roundActive = false;

        float player1Difference = Mathf.Abs(targetTime - player1Time);
        float player2Difference = Mathf.Abs(targetTime - player2Time);

        MoveHorseSmoothly(horse1, new Vector3((targetTime / 2) * (moveDistance / 8) / player1Difference, 0, 0), 1f);
        MoveHorseSmoothly(horse2, new Vector3((targetTime / 2) * (moveDistance / 8) / player2Difference, 0, 0), 1f);
        
        float effectDuration = 2f;

        if (player1Difference > 1)
        {
            track1.FlashRed();
        }
        else if (player1Difference < .2)
        {
            track1.FlashGreen();
        }

        if (player2Difference > 1)
        {
            track2.FlashRed();
        }
        else if (player2Difference < .2)
        {
            track2.FlashGreen();
        }

        if (player1Difference < player2Difference)
        {
            horse1.GetComponent<HorseEffects>().StartRainbowEffect(effectDuration);
            Debug.Log("Player 1 wins this round! Horse moved to: " + horse1.transform.position);
        }
        else if (player2Difference < player1Difference)
        {
            horse2.GetComponent<HorseEffects>().StartRainbowEffect(effectDuration);
            Debug.Log("Player 2 wins this round! Horse moved to: " + horse2.transform.position);
        }
        else
        {
            Debug.Log("It's a tie!");
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

        SceneManager.LoadScene("Start");
        yield return new WaitForSeconds(1f);

        //OLD WAY OF RESTARTING SCENE BEFORE WE HAD START SCREEN \/

        //float leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;
        //horse1.transform.position = new Vector3(leftEdge, -2f, 0);
        //horse2.transform.position = new Vector3(leftEdge, -3.1f, 0);

        //StartNewRound();
        SpawnHorses();
        StartNewRound();
    }

    private void MoveHorseSmoothly(GameObject horse, Vector3 moveOffset, float duration)
    {

        Player horseController = horse.GetComponent<Player>();
        if (horseController == null)
        {
            Debug.LogError("Horse has no Player script");
        }
        else
        {
            // Calculate the required velocity to reach moveOffset in the given duration
            Vector2 velocity = new Vector2(moveOffset.x / duration, moveOffset.y / duration);

            // Call Move with calculated velocity
            horseController.Move(velocity, duration);

        }

    }

    void PlayClockSound()
    {
        if (clockTickingSound != null && !clockTickingSound.isPlaying)
        {
            clockTickingSound.Play();
        }
    }

    void StopClockSound()
    {
        if (clockTickingSound != null && clockTickingSound.isPlaying)
        {
            clockTickingSound.Stop();
        }
    }

    void CheckStopClockSound()
    {
        if (player1Time > 0 && player2Time > 0)
        {
            StopClockSound();
        }
    }

    void SpawnPowerUp()
    {
        if (horse1 == null || horse2 == null) return; // ✅ Prevent errors

        GameObject trailingHorse = horse1.transform.position.x < horse2.transform.position.x ? horse1 : horse2;

        // Only spawn if the trailing horse HAS moved and WILL move next round
        if (trailingHorse.GetComponent<Player>().HasMovedThisRound)
        {
            GameObject powerUpPrefab = Random.value > 0.5f ? speedPowerUpPrefab : freezePowerUpPrefab;

            Vector3 spawnPosition = trailingHorse.transform.position + Vector3.right * 2.0f;
            GameObject powerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);

            // Reset power-up spawn flag when collected
            powerUp.GetComponent<Powerup>().OnCollected += () => powerUpSpawned = false;
        }
    }

    public void ResetPowerUpSpawn()
    {
        if (!powerUpSpawned) return;
        powerUpSpawned = false;
    }

        private void GetTracks()
    {
        track1 = GameObject.FindGameObjectWithTag("Player1Track").GetComponent<TrackEffects>();
        if(track1 == null)
        {
            Debug.LogError("Need a track1 and script");
        }

        track2 = GameObject.FindGameObjectWithTag("Player2Track").GetComponent<TrackEffects>();
        if (track2 == null)
        {
            Debug.LogError("Need a track2 and script");
        }
    }

    private void RainBowTrack()
    {

    }

}
