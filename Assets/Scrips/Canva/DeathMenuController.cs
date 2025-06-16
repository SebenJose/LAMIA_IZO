using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] private GameObject deathMenuPanel;

    [SerializeField] private TransitionControler transitionController;

    private string currentSceneName;

    [SerializeField] private float fadeInDuration = 0.5f;

    private CanvasGroup deathMenuCanvasGroup;

    void Awake()
    {
        if (deathMenuPanel != null)
        {
       
            deathMenuPanel.SetActive(false);

            deathMenuCanvasGroup = deathMenuPanel.GetComponent<CanvasGroup>();
            if (deathMenuCanvasGroup == null)
            {
                deathMenuCanvasGroup = deathMenuPanel.AddComponent<CanvasGroup>();
            }
            deathMenuCanvasGroup.alpha = 0f;
        }
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (transitionController == null)
        {
            transitionController = FindObjectOfType<TransitionControler>();
        }
    }
    public IEnumerator StartFadeInDeathMenu()
    {
        if (deathMenuPanel == null || deathMenuCanvasGroup == null)
        {
            Time.timeScale = 1f;
            yield break;
        }

        deathMenuPanel.SetActive(true);
        deathMenuCanvasGroup.alpha = 0f;

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            deathMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        deathMenuCanvasGroup.alpha = 1f;

        Time.timeScale = 0f;
    }

    public void HideDeathMenu()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);

            if (deathMenuCanvasGroup != null)
            {
                deathMenuCanvasGroup.alpha = 0f;
            }
            Time.timeScale = 1f;
        }
    }

    public void RestartLevel()
    {
        HideDeathMenu();
        if (transitionController != null)
        {
            transitionController.TransitionToScene(currentSceneName);
        }
        else
        {
            SceneManager.LoadScene(currentSceneName);
        }
        Time.timeScale = 1f;
    }

    public void BackToMainMenu(string mainMenuSceneName)
    {
        HideDeathMenu();
        if (transitionController != null)
        {
            transitionController.TransitionToScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        Time.timeScale = 1f;
    }
}