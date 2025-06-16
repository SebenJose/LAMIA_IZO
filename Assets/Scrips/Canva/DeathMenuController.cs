using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{
    // Refer�ncia ao painel do HUD de morte (o GameObject pai que cont�m todos os elementos do menu de morte)
    [SerializeField] private GameObject deathMenuPanel;

    // Refer�ncia ao seu TransitionControler para transi��es de cena
    [SerializeField] private TransitionControler transitionController;

    // Vari�vel para armazenar o nome da cena atual, para que possamos reinici�-la
    private string currentSceneName;

    // Dura��o da anima��o de fade-in para o menu de morte
    [SerializeField] private float fadeInDuration = 0.5f;

    // Refer�ncia ao CanvasGroup do painel de morte
    private CanvasGroup deathMenuCanvasGroup;

    void Awake()
    {
        if (deathMenuPanel != null)
        {
            // Inicialmente desativa o painel para que n�o apare�a antes da hora
            deathMenuPanel.SetActive(false);

            // Tenta obter o componente CanvasGroup do painel.
            // Se o painel n�o tiver um CanvasGroup, adiciona um automaticamente.
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
            Debug.LogError("ERRO: O campo 'Death Menu Panel' n�o foi atribu�do no Inspector do DeathMenuController. O menu de morte n�o funcionar� corretamente.");
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
                Debug.LogWarning("AVISO: TransitionControler n�o encontrado na cena. Por favor, atribua-o manualmente no Inspector do DeathMenuController para transi��es de cena.");
            }
        }
    }

    // NOVO M�TODO: Coroutine p�blica que gerencia o fade-in do menu de morte.
    // O PlayerController chamar� este m�todo usando StartCoroutine().
    public IEnumerator StartFadeInDeathMenu()
    {
        if (deathMenuPanel == null || deathMenuCanvasGroup == null)
        {
            Debug.LogError("Death Menu Panel ou CanvasGroup n�o atribu�do/encontrado no DeathMenuController. N�o � poss�vel iniciar o fade-in.");
            Time.timeScale = 1f; // Garante que o jogo n�o fique pausado
            yield break; // Sai da coroutine
        }

        // 1. Ativa o GameObject do painel de morte (ele ainda estar� transparente)
        deathMenuPanel.SetActive(true);
        deathMenuCanvasGroup.alpha = 0f; // Garante que o alpha comece em 0

        // 2. Anima o fade-in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime; // Usa Time.unscaledDeltaTime para anima��es de UI
            deathMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null; // Espera um frame
        }
        deathMenuCanvasGroup.alpha = 1f; // Garante que termine totalmente opaco

        // 3. Pausa o jogo AP�S a anima��o de fade-in terminar
        Time.timeScale = 0f;
        Debug.Log("Menu de Morte exibido com fade-in e jogo pausado.");
    }

    // Este m�todo � p�blico apenas para ser chamado por bot�es ou outros scripts que precisam esconder.
    // Ele n�o inicia coroutines.
    public void HideDeathMenu()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false); // Simplesmente desativa o GameObject

            // Garante que o alpha do CanvasGroup volte a 0 para o pr�ximo uso
            if (deathMenuCanvasGroup != null)
            {
                deathMenuCanvasGroup.alpha = 0f;
            }
            Time.timeScale = 1f; // Retoma o tempo do jogo
            Debug.Log("Menu de Morte escondido. Jogo retomado.");
        }
    }

    // Este m�todo ser� atribu�do ao bot�o "Reiniciar Fase"
    public void RestartLevel()
    {
        HideDeathMenu(); // Esconde o menu de morte antes da transi��o
        if (transitionController != null)
        {
            transitionController.TransitionToScene(currentSceneName);
        }
        else
        {
            Debug.LogWarning("TransitionControler n�o atribu�do ou encontrado. Carregando cena diretamente (sem transi��o).");
            SceneManager.LoadScene(currentSceneName);
        }
        Time.timeScale = 1f; // Garante que o tempo do jogo seja retomado
    }

    // Este m�todo ser� atribu�do ao bot�o "Voltar ao Menu Principal"
    public void BackToMainMenu(string mainMenuSceneName)
    {
        HideDeathMenu(); // Esconde o menu de morte antes da transi��o
        if (transitionController != null)
        {
            transitionController.TransitionToScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("TransitionControler n�o atribu�do ou encontrado. Carregando cena de menu principal diretamente (sem transi��o).");
            SceneManager.LoadScene(mainMenuSceneName);
        }
        Time.timeScale = 1f; // Garante que o tempo do jogo seja retomado
    }
}