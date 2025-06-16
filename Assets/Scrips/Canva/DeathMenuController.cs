using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{
    // Referência ao painel do HUD de morte (o GameObject pai que contém todos os elementos do menu de morte)
    [SerializeField] private GameObject deathMenuPanel;

    // Referência ao seu TransitionControler para transições de cena
    [SerializeField] private TransitionControler transitionController;

    // Variável para armazenar o nome da cena atual, para que possamos reiniciá-la
    private string currentSceneName;

    // Duração da animação de fade-in para o menu de morte
    [SerializeField] private float fadeInDuration = 0.5f;

    // Referência ao CanvasGroup do painel de morte
    private CanvasGroup deathMenuCanvasGroup;

    void Awake()
    {
        if (deathMenuPanel != null)
        {
            // Inicialmente desativa o painel para que não apareça antes da hora
            deathMenuPanel.SetActive(false);

            // Tenta obter o componente CanvasGroup do painel.
            // Se o painel não tiver um CanvasGroup, adiciona um automaticamente.
            deathMenuCanvasGroup = deathMenuPanel.GetComponent<CanvasGroup>();
            if (deathMenuCanvasGroup == null)
            {
                deathMenuCanvasGroup = deathMenuPanel.AddComponent<CanvasGroup>();
            }
            // Garante que o painel comece totalmente transparente ao ser carregado
            deathMenuCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("ERRO: O campo 'Death Menu Panel' não foi atribuído no Inspector do DeathMenuController. O menu de morte não funcionará corretamente.");
        }
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (transitionController == null)
        {
            transitionController = FindObjectOfType<TransitionControler>();
            if (transitionController == null)
            {
                Debug.LogWarning("AVISO: TransitionControler não encontrado na cena. Por favor, atribua-o manualmente no Inspector do DeathMenuController para transições de cena.");
            }
        }
    }

    // NOVO MÉTODO: Coroutine pública que gerencia o fade-in do menu de morte.
    // O PlayerController chamará este método usando StartCoroutine().
    public IEnumerator StartFadeInDeathMenu()
    {
        if (deathMenuPanel == null || deathMenuCanvasGroup == null)
        {
            Debug.LogError("Death Menu Panel ou CanvasGroup não atribuído/encontrado no DeathMenuController. Não é possível iniciar o fade-in.");
            Time.timeScale = 1f; // Garante que o jogo não fique pausado
            yield break; // Sai da coroutine
        }

        // 1. Ativa o GameObject do painel de morte (ele ainda estará transparente)
        deathMenuPanel.SetActive(true);
        deathMenuCanvasGroup.alpha = 0f; // Garante que o alpha comece em 0

        // 2. Anima o fade-in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime; // Usa Time.unscaledDeltaTime para animações de UI
            deathMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null; // Espera um frame
        }
        deathMenuCanvasGroup.alpha = 1f; // Garante que termine totalmente opaco

        // 3. Pausa o jogo APÓS a animação de fade-in terminar
        Time.timeScale = 0f;
        Debug.Log("Menu de Morte exibido com fade-in e jogo pausado.");
    }

    // Este método é público apenas para ser chamado por botões ou outros scripts que precisam esconder.
    // Ele não inicia coroutines.
    public void HideDeathMenu()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false); // Simplesmente desativa o GameObject

            // Garante que o alpha do CanvasGroup volte a 0 para o próximo uso
            if (deathMenuCanvasGroup != null)
            {
                deathMenuCanvasGroup.alpha = 0f;
            }
            Time.timeScale = 1f; // Retoma o tempo do jogo
            Debug.Log("Menu de Morte escondido. Jogo retomado.");
        }
    }

    // Este método será atribuído ao botão "Reiniciar Fase"
    public void RestartLevel()
    {
        HideDeathMenu(); // Esconde o menu de morte antes da transição
        if (transitionController != null)
        {
            transitionController.TransitionToScene(currentSceneName);
        }
        else
        {
            Debug.LogWarning("TransitionControler não atribuído ou encontrado. Carregando cena diretamente (sem transição).");
            SceneManager.LoadScene(currentSceneName);
        }
        Time.timeScale = 1f; // Garante que o tempo do jogo seja retomado
    }

    // Este método será atribuído ao botão "Voltar ao Menu Principal"
    public void BackToMainMenu(string mainMenuSceneName)
    {
        HideDeathMenu(); // Esconde o menu de morte antes da transição
        if (transitionController != null)
        {
            transitionController.TransitionToScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("TransitionControler não atribuído ou encontrado. Carregando cena de menu principal diretamente (sem transição).");
            SceneManager.LoadScene(mainMenuSceneName);
        }
        Time.timeScale = 1f; // Garante que o tempo do jogo seja retomado
    }
}