using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    // --- REFERÊNCIAS DO INSPECTOR ---
    [Header("Componentes do Menu")]
    [SerializeField] private GameObject pauseMenuPanel; // O painel principal do menu de pause
    [SerializeField] private Slider volumeSlider;       // O slider de volume da UI
    [SerializeField] private float fadeInDuration = 0.3f; // Duração do fade-in do menu

    [Header("Controle de Transição")]
    [SerializeField] private TransitionControler transitionController; // Seu controlador de transição de cena

    // --- CONTROLE DE ESTADO ---
    public static bool isPaused = false; // Variável estática para saber se o jogo está pausado

    private CanvasGroup pauseMenuCanvasGroup;
    private string mainMenuSceneName = "Menu"; // IMPORTANTE: Mude para o nome real da sua cena de menu

    void Awake()
    {
        // Garante que o painel está desativado no início
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
            Debug.LogError("ERRO: O 'Pause Menu Panel' não foi atribuído no Inspector!");
        }
    }

    void Start()
    {
        // Garante que o tempo está normal no início da cena
        Time.timeScale = 1f;
        isPaused = false;

        // Procura pelo TransitionController se não foi atribuído
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

            // Adiciona um listener para que o slider chame a função SetMusicVolume do AudioManager
            volumeSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
        }
        else
        {
            if (volumeSlider == null) Debug.LogWarning("AVISO: Slider de volume não atribuído no Inspector.");
            if (AudioManager.instance == null) Debug.LogWarning("AVISO: Instância do AudioManager não encontrada. O controle de volume não funcionará.");
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
            // Usa tempo não escalado para a animação funcionar mesmo com o jogo prestes a ser pausado
            timer += Time.unscaledDeltaTime;
            pauseMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        pauseMenuCanvasGroup.alpha = 1f;

        // Pausa o jogo DEPOIS que a animação termina
        Time.timeScale = 0f;
        Debug.Log("Jogo Pausado.");
    }

    public void ResumeGame()
    {
        if (!isPaused) return; // Evita resumir se não estiver pausado

        Time.timeScale = 1f; // Retoma o tempo do jogo PRIMEIRO
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        pauseMenuCanvasGroup.alpha = 0f; // Reseta o alpha para o próximo uso
        Debug.Log("Jogo Retomado.");
    }

    // --- MÉTODOS PARA OS BOTÕES (Atribuir no Inspector) ---

    // Atribua este método ao OnClick() do botão "Voltar ao Jogo"
    public void OnResumeButtonPressed()
    {
        ResumeGame();
    }

    // Atribua este método ao OnClick() do botão "Voltar ao Menu"
    public void OnBackToMenuPressed()
    {
        // Garante que o jogo está despausado antes de mudar de cena
        ResumeGame();

        if (transitionController != null)
        {
            transitionController.TransitionToScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("TransitionController não encontrado. Carregando cena de menu diretamente.");
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}