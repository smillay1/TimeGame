using UnityEngine;
using System;

public class Powerup : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, FreezeOpponent }
    public PowerUpType powerUpType;

    private bool canBeCollected = false;
    private GameManager gameManager;

    //  Add an event for when the power-up is collected
    public event Action OnCollected;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in scene! Make sure it exists.");
        }
        Invoke("EnableCollection", 0.5f); // Prevent instant pickup
    }

    void EnableCollection()
    {
        canBeCollected = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player horse = other.GetComponent<Player>();

        if (horse != null && canBeCollected)
        {
            if (powerUpType == PowerUpType.SpeedBoost)
            {
                horse.ActivateSpeedBoost();
            }
            else if (powerUpType == PowerUpType.FreezeOpponent)
            {
                horse.FreezeOpponent();
            }

            OnCollected?.Invoke(); // Notify GameManager before destroying
            if (gameManager != null) // Double-check GameManager exists before calling method
            {
                gameManager.ResetPowerUpSpawn();
            }
            Destroy(gameObject);
        }
    }
}
