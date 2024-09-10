using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SplashScreen : MonoBehaviour
{
    public TextMeshProUGUI splashText;
    public float initialDelay = 2f; 
    public float fadeDuration = 1f;
    public float displayTime = 5f; 
    public float finalDelay = 1f; 
    public int menuSceneIndex = 1; 


    void Start()
    {
        if (splashText == null)
        {
            Debug.LogError("SplashText is not assigned!");
            return;
        }

      
        SetAlpha(0);
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
     
        yield return new WaitForSeconds(initialDelay);

        // Fade in
        yield return FadeIn();

      
        yield return new WaitForSeconds(displayTime);

    
        yield return FadeOut();

  
        yield return new WaitForSeconds(finalDelay);

   
        SceneManager.LoadScene(menuSceneIndex);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1); 
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0); 
    }

    private void SetAlpha(float alpha)
    {
        if (splashText != null)
        {
            Color color = splashText.color;
            color.a = alpha;
            splashText.color = color;
        }
    }
}
