using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    // --- REFER�NCIAS DO INSPECTOR ---
    [Header("Componentes do Menu")]
    [SerializeField] private GameObject pauseMenuPanel; // O painel principal do menu de pause
    [SerializeField] private Slider volumeSlider;       // O slider de volume da UI
    [SerializeField] private float fadeInDuration = 0.3f; // Dura��o do fade-in do menu

    [Header("Controle de Transi��o")]
    [SerializeField] private TransitionControler transitionController; // Seu controlador de transi��o de cena

    // --- CONTROLE DE ESTADO ---
    public static bool isPaused = false; // Vari�vel est�tica para saber se o jogo est� pausado

    private CanvasGroup pauseMenuCanvasGroup;
    private string mainMenuSceneName = "Menu"; // IMPORTANTE: Mude para o nome real da sua cena de menu

    void Awake()
    {
        // Garante que o painel est� desativado no in�cio
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);

            // Pega ou adiciona o CanvasGroup para o efeito de fade
            pauseMenuCanvasGroup = pauseMenuPanel.GetComponent<CanvasGroup>();
            if (pauseMenuCanvasGroup == null)
            {
                pauseMenuCanvasGroup = pauseMenuPanel.AddComponent<CanvasGroup>();
            }
            pauseMenuCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("ERRO: O 'Pause Menu Panel' n�o foi atribu�do no Inspector!");
        }
    }

    void Start()
    {
        // Garante que o tempo est� normal no in�cio da cena
        Time.timeScale = 1f;
        isPaused = false;

        // Procura pelo TransitionController se n�o foi atribu�do
        if (transitionController == null)
        {
            transitionController = FindObjectOfType<TransitionControler>();
        }

        // Configura o slider de volume
        SetupVolumeSlider();
    }

    void Update()
    {
        // Detecta se a tecla ESC foi pressionada
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
            // Define o valor inicial do slider com base no volume salvo
            volumeSlider.value = AudioManager.instance.GetMusicVolume();

            // Adiciona um listener para que o slider chame a fun��o SetMusicVolume do AudioManager
            volumeSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
        }
        else
        {
            if (volumeSlider == null) Debug.LogWarning("AVISO: Slider de volume n�o atribu�do no Inspector.");
            if (AudioManager.instance == null) Debug.LogWarning("AVISO: Inst�ncia do AudioManager n�o encontrada. O controle de volume n�o funcionar�.");
        }
    }

    private void PauseGame()
    {
        if (isPaused) return; // Evita pausar duas vezes

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
            // Usa tempo n�o escalado para a anima��o funcionar mesmo com o jogo prestes a ser pausado
            timer += Time.unscaledDeltaTime;
            pauseMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        pauseMenuCanvasGroup.alpha = 1f;

        // Pausa o jogo DEPOIS que a anima��o termina
        Time.timeScale = 0f;
        Debug.Log("Jogo Pausado.");
    }

    public void ResumeGame()
    {
        if (!isPaused) return; // Evita resumir se n�o estiver pausado

        Time.timeScale = 1f; // Retoma o tempo do jogo PRIMEIRO
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        pauseMenuCanvasGroup.alpha = 0f; // Reseta o alpha para o pr�ximo uso
        Debug.Log("Jogo Retomado.");
    }

    // --- M�TODOS PARA OS BOT�ES (Atribuir no Inspector) ---

    // Atribua este m�todo ao OnClick() do bot�o "Voltar ao Jogo"
    public void OnResumeButtonPressed()
    {
        ResumeGame();
    }

    // Atribua este m�todo ao OnClick() do bot�o "Voltar ao Menu"
    public void OnBackToMenuPressed()
    {
        // Garante que o jogo est� despausado antes de mudar de cena
        ResumeGame();

        if (transitionController != null)
        {
            transitionController.TransitionToScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("TransitionController n�o encontrado. Carregando cena de menu diretamente.");
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}