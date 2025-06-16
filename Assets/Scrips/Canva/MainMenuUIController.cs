using UnityEngine;
using UnityEngine.UI; // Para Slider, Button
using UnityEngine.SceneManagement; // Para carregar cenas

public class MainMenuUIController : MonoBehaviour
{
    [Header("Componentes UI")]
    public Slider musicVolumeSlider; // Arraste seu Slider de m�sica aqui
    public GameObject mainMenuPanel; // Arraste o GameObject que cont�m os bot�es do menu principal
    public GameObject optionsMenuPanel; // Arraste o GameObject que cont�m os elementos do menu de op��es
    public GameObject creditsPanel; // NOVO: Arraste o GameObject que cont�m os elementos da tela de cr�ditos

    // Certifique-se de ter o AudioManager em sua cena e configurado!

    void Start()
    {
        // Inicializa o slider de volume da m�sica
        if (musicVolumeSlider != null && AudioManager.instance != null)
        {
            musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
            // Adiciona o listener para o slider, caso n�o tenha sido adicionado no Inspector
            // � bom adicionar via c�digo tamb�m para garantir
            musicVolumeSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
        }

        // Garante que o menu principal esteja ativo e os outros inativos ao iniciar
        ShowMainMenu();
    }

    // --- M�todos para Bot�es ---

    // Chamado pelo bot�o "PLAY"
    public void OnPlayButton()
    {
        Debug.Log("Bot�o PLAY clicado!");
        // Certifique-se de que a cena "Introducao" esteja nas Build Settings (File > Build Settings)
        SceneManager.LoadScene("Introducion");
    }

    // Chamado pelo bot�o "OP��ES"
    public void OnOptionsButton()
    {
        Debug.Log("Bot�o OP��ES clicado!");
        ShowOptionsMenu();
    }

    // NOVO: Chamado pelo bot�o "CR�DITOS" (no menu principal, por exemplo)
    public void OnCreditsButton()
    {
        Debug.Log("Bot�o CR�DITOS clicado!");
        ShowCreditsPanel();
    }

    // Chamado pelo bot�o "VOLTAR" (dentro do menu de op��es OU da tela de cr�ditos)
    public void OnBackButton()
    {
        Debug.Log("Bot�o VOLTAR clicado!");
        ShowMainMenu(); // Sempre volta para o menu principal
    }

    // Chamado pelo bot�o "SAIR" (se voc� tiver um)
    public void OnExitButton()
    {
        Debug.Log("Bot�o SAIR clicado!");
        Application.Quit(); // Fecha o aplicativo (n�o funciona no editor, apenas em builds)

        // Se estiver no editor, voc� pode adicionar um log para saber que funcionaria
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- M�todos para Gerenciar Telas ---

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
        // NOVO: Desativa o painel de cr�ditos ao mostrar o menu principal
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
        // NOVO: Desativa o painel de cr�ditos ao mostrar o menu de op��es
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    // NOVO: M�todo para mostrar a tela de cr�ditos
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