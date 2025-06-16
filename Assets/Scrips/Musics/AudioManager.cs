using UnityEngine;
using UnityEngine.Audio; // Necessário para AudioMixer
using UnityEngine.SceneManagement; // Necessário para SceneManager
using System.Collections.Generic; // Necessário para Dictionary

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Instância Singleton

    public AudioMixer mainMixer; // Arraste seu MainAudioMixer aqui
    public AudioSource musicSource; // O AudioSource que tocará a música

    // Um dicionário para mapear nomes de cenas para seus AudioClips
    public List<SceneMusicEntry> sceneMusicMapping = new List<SceneMusicEntry>();

    [System.Serializable]
    public class SceneMusicEntry
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    private float currentMusicVolume; // Para armazenar o volume atual do mixer

    void Awake()
    {
        // Implementação do Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre cenas
        }
        else
        {
            Destroy(gameObject); // Destrói duplicatas
            return; // Sai do Awake
        }

        // Garante que as referências não são nulas
        if (musicSource == null) musicSource = GetComponent<AudioSource>();
        if (mainMixer == null) Debug.LogError("AudioManager: MainAudioMixer não atribuído!");

        // Carrega o volume salvo, se houver
        LoadVolume();
    }

    void OnEnable()
    {
        // Assina o evento de carregamento de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Desassina o evento para evitar vazamentos de memória
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamado sempre que uma nova cena é carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        // Busca a música correspondente à cena
        foreach (var entry in sceneMusicMapping)
        {
            if (entry.sceneName == sceneName)
            {
                clipToPlay = entry.musicClip;
                break;
            }
        }

        // Se encontrou uma música e ela é diferente da atual, toca
        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
        else if (clipToPlay == null && musicSource.isPlaying)
        {
            // Se a cena não tem música definida, para a música atual
            musicSource.Stop();
        }
        else if (clipToPlay != null && !musicSource.isPlaying)
        {
            // Se encontrou a mesma música e não está tocando (ex: ao voltar para a cena)
            musicSource.Play();
        }
    }

    // Método para ser chamado pelo Slider UI
    public void SetMusicVolume(float volume)
    {
        // O volume do mixer é logarítmico, então convertemos o valor linear do slider
        // minSliderValue (0) corresponde a -80dB (silêncio)
        // maxSliderValue (1) corresponde a 0dB (volume máximo)
        if (volume <= 0.001f) // Para evitar log(0) e garantir mudo real
        {
            mainMixer.SetFloat("MusicVolume", -80f); // -80dB é geralmente mudo
        }
        else
        {
            // Converte o volume linear (0 a 1) para dB (logarítmico)
            // Math.Log10(volume) * 20 -> mapeia 0-1 para aproximadamente -80 a 0 dB
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
        currentMusicVolume = volume; // Armazena o valor linear do slider
        SaveVolume(volume); // Salva o volume
    }

    // Obtém o volume atual (linear) do mixer
    public float GetMusicVolume()
    {
        float volumeInDb;
        mainMixer.GetFloat("MusicVolume", out volumeInDb);

        // Converte de dB para linear (0-1) para o slider
        if (volumeInDb <= -79f) // Se for muito baixo (-80dB ou perto), é considerado 0
        {
            return 0f;
        }
        else
        {
            return Mathf.Pow(10, volumeInDb / 20f);
        }
    }

    private void SaveVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save(); // Garante que é salvo no disco
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
            SetMusicVolume(savedVolume); // Aplica o volume salvo
        }
        else
        {
            SetMusicVolume(1.0f); // Volume padrão se não houver um salvo
        }
    }
}