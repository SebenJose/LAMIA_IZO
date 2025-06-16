using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    [Header("Componentes do Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float fadeInDuration = 0.3f;

    [Header("Controle de Transição")]
    [SerializeField] private TransitionControler transitionController;

    public static bool isPaused = false;

    private CanvasGroup pauseMenuCanvasGroup;
    private string mainMenuSceneName = "Menu";

    void Awake()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);

            pauseMenuCanvasGroup = pauseMenuPanel.GetComponent<CanvasGroup>();
            if (pauseMenuCanvasGroup == null)
            {
                pauseMenuCanvasGroup = pauseMenuPanel.AddComponent<CanvasGroup>();
            }
            pauseMenuCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("ERRO: O 'Pause Menu Panel' não foi atribuído no Inspector!");
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (transitionController == null)
        {
            transitionController = FindObjectOfType<TransitionControler>();
        }

        SetupVolumeSlider();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void SetupVolumeSlider()
    {
        if (volumeSlider != null && AudioManager.instance != null)
        {
            volumeSlider.value = AudioManager.instance.GetMusicVolume();

            volumeSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
        }
    }

    private void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        StartCoroutine(FadeInPauseMenu());
    }

    private IEnumerator FadeInPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        pauseMenuCanvasGroup.alpha = 0f;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            pauseMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        pauseMenuCanvasGroup.alpha = 1f;

        Time.timeScale = 0f;
        Debug.Log("Jogo Pausado.");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        pauseMenuCanvasGroup.alpha = 0f;
        Debug.Log("Jogo Retomado.");
    }


    public void OnResumeButtonPressed()
    {
        ResumeGame();
    }

    public void OnBackToMenuPressed()
    {
        ResumeGame();

        if (transitionController != null)
        {
            transitionController.TransitionToScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}