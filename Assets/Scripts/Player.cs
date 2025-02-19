using UnityEngine;
using System.Collections;

public abstract class Player : MonoBehaviour
{
    protected Rigidbody2D rb; // If using physics-based movement

    private bool isMoving = false;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on " + gameObject.name);
        }
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
        isMoving = true;
        StartCoroutine(MoveCoroutine(velocity, duration));
        
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

}
