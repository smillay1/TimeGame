using UnityEngine;
using System.Collections;

public abstract class Player : MonoBehaviour
{
    protected Rigidbody2D rb; // If using physics-based movement

    private bool isMoving = false;
    private bool isFrozen = false;
    private bool speedBoostActive = false;
    private float originalSpeed;
    private bool hasMovedThisRound = false;
    public bool HasMovedThisRound => hasMovedThisRound; 
    private int moveCount = 0;
    public int MoveCount { get; private set; }
    private float boostDuration = 4f;
    private float boostedSpeedMultiplier = 4.0f;
    private bool isBoosted = false;
    
    public float moveDistance = 1.5f;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on " + gameObject.name);
        }

        originalSpeed = moveDistance; // Store the default movement distance
    }

    public void Update()
    {
        if (isMoving)
        {
            Animate();
        }
    }

    protected abstract void Animate();
    protected abstract void EndAnimate();

    public void Move(Vector2 velocity, float duration)
    {

        Debug.Log($"{gameObject.name} is attempting to move.");

        if (isFrozen)
        {
            Debug.Log(gameObject.name + " is frozen!");
            return; // Stop movement if frozen
        }

        // Apply speed boost but only for this move
        if (isBoosted)
        {
            velocity *= boostedSpeedMultiplier; // Double movement speed
            duration *= boostedSpeedMultiplier;
        }
        hasMovedThisRound = true;
        Debug.Log($"{gameObject.name} has moved this round!");

        isMoving = true;
        StartCoroutine(MoveCoroutine(velocity, duration));
        
        FindObjectOfType<GameManager>().CheckPowerUpSpawnCondition();
    }

    private IEnumerator MoveCoroutine(Vector2 velocity, float duration)
    {
        Debug.Log("Moving");
        rb.linearVelocity = velocity;
        yield return new WaitForSeconds(duration); // Wait for the duration

        rb.linearVelocity = Vector2.zero; // Stop movement after duration
        EndAnimate();
        isMoving = false;
    }

    public void ActivateSpeedBoost()
    {
        if (!isBoosted)
        {
            Debug.Log($"{gameObject.name} is now SUPER FAST! ⚡");
            isBoosted = true;
            speedBoostActive = true;
            StartCoroutine(ResetSpeedAfterDelay());
        }
    }

    private IEnumerator ResetSpeedAfterDelay()
    {
        yield return new WaitForSeconds(boostDuration);
        isBoosted = false;
        speedBoostActive = false;
        Debug.Log($"{gameObject.name} speed boost has ended. 🏇");
    }

    public void FreezeOpponent()
    {
        Player opponent = FindOpponent();
        if (opponent != null)
        {
            opponent.StartCoroutine(opponent.FreezeCoroutine()); // ✅ Start freeze effect
        }
    }

    private IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        Debug.Log(gameObject.name + " is frozen for 6 seconds!");

        rb.linearVelocity = Vector2.zero;
        
        yield return new WaitForSeconds(6f); // Wait for freeze duration

        isFrozen = false;
        Debug.Log(gameObject.name + " is no longer frozen!");
    }

    private Player FindOpponent()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players)
        {
            if (p != this) return p; // Return the other player
        }
        return null;
    }

    public void ResetMoveCount()
    {
        MoveCount = 0; // Resets the number of moves correctly
        hasMovedThisRound = false;
    }

    public void ResetMovement()
    {
        hasMovedThisRound = false; // Reset movement tracking
    }

    public void IncrementMoveCount()
    {
        MoveCount++;  // Increases only for the winner
        Debug.Log(gameObject.name + " move count increased to " + MoveCount);
    }

}
