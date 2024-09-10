using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Image fadePanel; // Reference to the Image component of the black panel
    public float fadeDuration = 1f; // Duration for the fade out effect
    public float initialDelay = 1f; // Delay before starting the fade effect

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.transform.parent.gameObject.SetActive(true);
            StartCoroutine(FadeOutPanel());
        }
    }

    private IEnumerator FadeOutPanel()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(initialDelay);

        Color color = fadePanel.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            color.a = alpha;
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0; // Ensure the panel is fully transparent at the end
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(false); // Optionally disable the panel
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("None"); // Replace "None" with the scene name or index
    }
}
