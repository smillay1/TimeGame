using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public float moveSpeed = 1f;
    public float fadeDuration = 1f;

    void Start()
    {
        Destroy(gameObject, fadeDuration); // Auto-destroy after fading
    }

    public void SetText(float value)
    {
        textMesh.text = (value >= 0 ? "+" : "") + value.ToString("F1") + "s"; // Format + or - sign
        StartCoroutine(FloatAndFadeOut());
    }

    private IEnumerator FloatAndFadeOut()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, 1.5f, 0); // Moves upward

        float elapsedTime = 0f;
        Color textColor = textMesh.color;

        while (elapsedTime < fadeDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / fadeDuration);
            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, 1 - (elapsedTime / fadeDuration)); // Fade out
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Destroy when faded out
    }
}
