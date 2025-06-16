using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject creditsPanel;


    void Start()
    {
        if (musicVolumeSlider != null && AudioManager.instance != null)
        {
            musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
            musicVolumeSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
        }

        ShowMainMenu();
    }


    public void OnPlayButton()
    {
        SceneManager.LoadScene("Introducion");
    }

    public void OnOptionsButton()
    {
        ShowOptionsMenu();
    }

    public void OnCreditsButton()
    {
        ShowCreditsPanel();
    }

    public void OnBackButton()
    {
        ShowMainMenu(); // Sempre volta para o menu principal
    }

    public void OnExitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

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
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

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