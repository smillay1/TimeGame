using UnityEngine;
using System.Collections;

public class TrackEffects : MonoBehaviour
{
    public float speed = 1.0f; // Speed of rainbow transition
    private SpriteRenderer spriteRenderer;
    private Coroutine rainbowCoroutine;
    public Color BaseColor = new Color(162f / 255f, 127f / 255f, 93f / 255f);

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartRainbowEffect(float duration)
    {
        if (rainbowCoroutine != null)
        {
            StopCoroutine(rainbowCoroutine);
        }
        rainbowCoroutine = StartCoroutine(RainbowEffectCoroutine(duration));
    }

    private IEnumerator RainbowEffectCoroutine(float rainbowDuration)
    {
        float timer = 0f;
        while (timer < rainbowDuration)
        {
            float hue = Mathf.Repeat(Time.time * speed, 1.0f);
            spriteRenderer.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            timer += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = BaseColor; // Revert to previous color
    }

    public void FlashGreen()
    {
        Color targetColor = new Color(12f / 255f, 172f / 255f, 19f / 255f); // Normalized green color
        StartCoroutine(FlashCoroutine(1, targetColor));
    }

    public void FlashRed()
    {
        Color targetColor = new Color(185f / 255f, 14f / 255f, 10f / 255f); // Normalized green color
        StartCoroutine(FlashCoroutine(1, targetColor));
    }

    private IEnumerator FlashCoroutine(float flashDuration, Color targetColor)
    {
        float segmentDuration = flashDuration / 3f;
        float elapsedTime = 0f;

        // Smooth transition to green
        while (elapsedTime < segmentDuration)
        {
            spriteRenderer.color = Color.Lerp(BaseColor, targetColor, elapsedTime / segmentDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = targetColor; // Ensure exact green color

        // Stay at green for the middle third of the time
        yield return new WaitForSeconds(segmentDuration);

        elapsedTime = 0f;

        // Smooth transition back to base color
        while (elapsedTime < segmentDuration)
        {
            spriteRenderer.color = Color.Lerp(targetColor, BaseColor, elapsedTime / segmentDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = BaseColor; // Ensure exact base color
    }


}

