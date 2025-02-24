using System.Collections;
using UnityEngine;

public class HorseEffects : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color[] rainbowColors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta };
    private float colorChangeSpeed = 0.2f;
    private bool isGlowing = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on " + gameObject.name);
        }
    }

    public void StartRainbowEffect(float duration)
    {
        if (!isGlowing)
        {
            StartCoroutine(RainbowEffect(duration));
        }
    }

    private IEnumerator RainbowEffect(float duration)
    {
        isGlowing = true;
        float elapsedTime = 0f;
        int colorIndex = 0;

        while (elapsedTime < duration)
        {
            spriteRenderer.color = rainbowColors[colorIndex];
            colorIndex = (colorIndex + 1) % rainbowColors.Length;

            yield return new WaitForSeconds(colorChangeSpeed); 
            elapsedTime += colorChangeSpeed;
        }

        spriteRenderer.color = Color.white;
        isGlowing = false;
    }
}
