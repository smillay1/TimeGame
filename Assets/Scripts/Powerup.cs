using UnityEngine;
using System;

public class Powerup : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, FreezeOpponent }
    public PowerUpType powerUpType;

    private bool canBeCollected = false;

    //  Add an event for when the power-up is collected
    public event Action OnCollected;

    void Start()
    {
    {
        Invoke("EnableCollection", 0.5f); // Prevent instant pickup
    }
    }

    void EnableCollection()
    {
        canBeCollected = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player horse = other.GetComponent<Player>();

        if (horse != null && canBeCollected && horse.HasMovedThisRound)
        {
            if (powerUpType == PowerUpType.SpeedBoost)
            {
                horse.ActivateSpeedBoost();
            }
            else if (powerUpType == PowerUpType.FreezeOpponent)
            {
                horse.FreezeOpponent();
            }

            OnCollected?.Invoke(); //  Notify GameManager before destroying
            FindObjectOfType<GameManager>().ResetPowerUpSpawn(); //  Ensure reset
            Destroy(gameObject);
        }
    }
}
