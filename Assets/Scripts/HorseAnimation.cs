using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseAnimation : Player
{
    [SerializeField] private List<Sprite> runningSprites;  // List of sprites for animation
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private float animationSpeed = 0.1f; // Speed of the animation

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float animationTimer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on " + gameObject.name);
        }
    }

    protected override void Animate()
    {
        // Debug.Log("animating");
        if (runningSprites.Count == 0 || spriteRenderer == null)
            return;

        animationTimer += Time.deltaTime;
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % runningSprites.Count;
            spriteRenderer.sprite = runningSprites[currentFrame];
        }
    }

    protected override void EndAnimate()
    {
        spriteRenderer.sprite = standingSprite;
    }
}
