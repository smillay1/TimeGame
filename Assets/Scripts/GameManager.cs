//General script handling game functionality

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI targetTimeText;
    public static GameManager Instance;

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
    public AudioSource bigSparkleSound;

    public AudioSource smallSparkleSound;
    public AudioSource badSound;

    public GameObject speedPowerUpPrefab;
    public GameObject freezePowerUpPrefab;
    private bool powerUpSpawned = false;
    public GameObject squareBurstEffect;

    void Start()
    {
        SpawnHorses();
        StartNewRound();
        GetTracks();
        
        if (Instance == null) {
            Instance = this;
        }
        
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
    }
    
    //Spawns horses in at horse spawner at left side of screen
    void SpawnHorses()
    {
        Debug.Log("Spawning horses...");

        if (horse1Prefab == null || horse2Prefab == null)
        {
            Debug.LogError("Horse prefabs are not assigned!");
            return;
        }

        Vector3 horse1Spawn = GameObject.FindGameObjectWithTag("Player1Spawner").transform.position;
        Vector3 horse2Spawn = GameObject.FindGameObjectWithTag("Player2Spawner").transform.position;

        horse1 = Instantiate(horse1Prefab, horse1Spawn, Quaternion.identity);
        horse2 = Instantiate(horse2Prefab, horse2Spawn, Quaternion.identity);

        Debug.Log("Horse 1 spawned at: " + horse1.transform.position);
        Debug.Log("Horse 2 spawned at: " + horse2.transform.position);
    }

    //Begin new round of guessing for both players
    void StartNewRound()
    {
        Debug.Log("StartNewRound() is running! Resetting target time.");
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
        
        startTime = Time.time;
        player1Time = 0f;
        player2Time = 0f;

        roundActive = true;
        PlayClockSound();
    }

    //Move each horse forward in proportion to how close their guess is to the actual time.
    void DetermineRoundWinner()
    {
        roundActive = false;

        float player1Difference = Mathf.Abs(targetTime - player1Time);
        float player2Difference = Mathf.Abs(targetTime - player2Time);

        MoveHorseSmoothly(horse1, new Vector3((targetTime / 2) * (moveDistance / 8) / player1Difference, 0, 0), 1f);
        MoveHorseSmoothly(horse2, new Vector3((targetTime / 2) * (moveDistance / 8) / player2Difference, 0, 0), 1f);
        
        float effectDuration = 2f;

        bool player1Wins = player1Difference < player2Difference;
        bool player2Wins = player2Difference < player1Difference;

        if (player1Difference > 1.5)
        {
            track1.FlashRed();
            badSound.Play();
        }
        if (player1Difference < .3)
        {
            track1.FlashGreen();
            smallSparkleSound.Play();
        }

        if (player1Difference < .2)
        {
            Instantiate(squareBurstEffect, horse1.transform.position + new Vector3(-0.5f, 0, 0), Quaternion.identity);
            bigSparkleSound.Play();
        }

        if (player2Difference > 1.5)
        {
            track2.FlashRed();
            badSound.Play();
        }
        if (player2Difference < .3)
        {
            track2.FlashGreen();
            smallSparkleSound.Play();
        }

        if (player2Difference < .2)
        {
            Instantiate(squareBurstEffect, horse2.transform.position + new Vector3(-0.5f, 0, 0), Quaternion.identity);
            Debug.Log("ZOOOOOOOOM");
            bigSparkleSound.Play();
        }

        if (player1Difference < player2Difference)
        {
            horse1.GetComponent<HorseEffects>().StartRainbowEffect(effectDuration);
            horse1.GetComponent<Player>().IncrementMoveCount();
            Debug.Log("Player 1 wins this round! Horse moved to: " + horse1.transform.position);
        } else if (player2Difference < player1Difference)
        {
            horse2.GetComponent<HorseEffects>().StartRainbowEffect(effectDuration);
            horse2.GetComponent<Player>().IncrementMoveCount();
            Debug.Log("Player 2 wins this round! Horse moved to: " + horse2.transform.position);
        } else
        {
            Debug.Log("It's a tie!");
        }

        StartCoroutine(WaitBeforeNextRound());

    }

    IEnumerator WaitBeforeNextRound()
    {
        Debug.Log("Waiting before next round...");
        yield return new WaitForSeconds(2f);

        Debug.Log("Starting new round...");
        StartNewRound();
    }

    //Once a player has won the round, move to the next
    public IEnumerator RestartGame(EndManager.Player winner)
    {
        if (horse1.transform.position.x > horse2.transform.position.x)
        {
            targetTimeText.text = "Player 1 Wins!";
            
        }

        if (horse1.transform.position.x < horse2.transform.position.x)
        {
            targetTimeText.text = "Player 2 Wins!";
            
        }

        yield return new WaitForSeconds(2f);

        EndManager.AddWinner(winner);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
    //Move horse smoothly instead of teleporting to next position
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

    //Clock ticking during guessing
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

    //Checks if players can get a powerup - when they are losing by 3 guesses or more
    public void CheckPowerUpSpawnCondition()
    {
        if (horse1 != null && horse2 != null)
        {
            int moveDifference = Mathf.Abs(horse1.GetComponent<Player>().MoveCount - horse2.GetComponent<Player>().MoveCount);
            Debug.Log($"Move Difference: {moveDifference}");

            if (moveDifference >= 3 && !powerUpSpawned)
            {
                SpawnPowerUp();
                powerUpSpawned = true;
                horse1.GetComponent<Player>().ResetMoveCount();
                horse2.GetComponent<Player>().ResetMoveCount();
            }
        }
    }

    //Spawns powerups to trailing players
    void SpawnPowerUp()
    {
        if (horse1 == null || horse2 == null) {
            Debug.Log("SpawnPowerUp() aborted: One of the horses is null.");
            return; // ✅ Prevent errors
        }

        int moveDifference = Mathf.Abs(horse1.GetComponent<Player>().MoveCount - horse2.GetComponent<Player>().MoveCount);
        Debug.Log($"🟡 Move Difference: {moveDifference}, Power-up spawned: {powerUpSpawned}");

        if (moveDifference < 3 || powerUpSpawned) 
        {
            Debug.Log("Conditions not met for spawning power-up.");
            return;
        }

        GameObject trailingHorse = horse1.transform.position.x < horse2.transform.position.x ? horse1 : horse2;

        if (!trailingHorse.GetComponent<Player>().HasMovedThisRound)
        {
            Debug.Log("Power-up not spawning: trailing horse has not moved yet.");
            return;
        }

        GameObject powerUpPrefab = Random.value > 0.5f ? speedPowerUpPrefab : freezePowerUpPrefab;

        if (powerUpPrefab == null)
        {
            Debug.LogError("Power-up prefab is null! Make sure it is assigned in the Inspector.");
            return;
        }

        Vector3 spawnPosition = trailingHorse.transform.position + Vector3.right * 2.0f;
        Debug.Log($"✅ Spawning power-up: {powerUpPrefab.name} at {spawnPosition}");

        GameObject powerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        powerUp.GetComponent<Powerup>().OnCollected += () => powerUpSpawned = false; // Reset power-up spawn flag when collected

        powerUpSpawned = true; // Prevent spawning another power-up until collected
    }

    public void ResetPowerUpSpawn()
    {
        if (!powerUpSpawned) return;
        powerUpSpawned = false;
    }
    //Tracks for lighting up when players preform well and poorly
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
