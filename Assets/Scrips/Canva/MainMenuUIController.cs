using UnityEngine;
using UnityEngine.UI; // Para Slider, Button
using UnityEngine.SceneManagement; // Para carregar cenas

public class MainMenuUIController : MonoBehaviour
{
    [Header("Componentes UI")]
    public Slider musicVolumeSlider; // Arraste seu Slider de música aqui
    public GameObject mainMenuPanel; // Arraste o GameObject que contém os botões do menu principal
    public GameObject optionsMenuPanel; // Arraste o GameObject que contém os elementos do menu de opções
    public GameObject creditsPanel; // NOVO: Arraste o GameObject que contém os elementos da tela de créditos

    // Certifique-se de ter o AudioManager em sua cena e configurado!

    void Start()
    {
        // Inicializa o slider de volume da música
        if (musicVolumeSlider != null && AudioManager.instance != null)
        {
            musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
            // Adiciona o listener para o slider, caso não tenha sido adicionado no Inspector
            // É bom adicionar via código também para garantir
            musicVolumeSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
        }

        // Garante que o menu principal esteja ativo e os outros inativos ao iniciar
        ShowMainMenu();
    }

    // --- Métodos para Botões ---

    // Chamado pelo botão "PLAY"
    public void OnPlayButton()
    {
        Debug.Log("Botão PLAY clicado!");
        // Certifique-se de que a cena "Introducao" esteja nas Build Settings (File > Build Settings)
        SceneManager.LoadScene("Introducion");
    }

    // Chamado pelo botão "OPÇÕES"
    public void OnOptionsButton()
    {
        Debug.Log("Botão OPÇÕES clicado!");
        ShowOptionsMenu();
    }

    // NOVO: Chamado pelo botão "CRÉDITOS" (no menu principal, por exemplo)
    public void OnCreditsButton()
    {
        Debug.Log("Botão CRÉDITOS clicado!");
        ShowCreditsPanel();
    }

    // Chamado pelo botão "VOLTAR" (dentro do menu de opções OU da tela de créditos)
    public void OnBackButton()
    {
        Debug.Log("Botão VOLTAR clicado!");
        ShowMainMenu(); // Sempre volta para o menu principal
    }

    // Chamado pelo botão "SAIR" (se você tiver um)
    public void OnExitButton()
    {
        Debug.Log("Botão SAIR clicado!");
        Application.Quit(); // Fecha o aplicativo (não funciona no editor, apenas em builds)

        // Se estiver no editor, você pode adicionar um log para saber que funcionaria
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- Métodos para Gerenciar Telas ---

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(false);
        }
        // NOVO: Desativa o painel de créditos ao mostrar o menu principal
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void ShowOptionsMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(true);
        }
        // NOVO: Desativa o painel de créditos ao mostrar o menu de opções
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    // NOVO: Método para mostrar a tela de créditos
    public void ShowCreditsPanel()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(false);
        }
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }
}