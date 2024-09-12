using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
    public Image fadePanel; 
    public float fadeDuration = 1f; 
    public float initialDelay = 1f; 
    public int menuSceneIndex = 1; 
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

        color.a = 0; 
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(false); 
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }
}
