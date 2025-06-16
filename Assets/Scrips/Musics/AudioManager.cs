using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mainMixer;
    public AudioSource musicSource;

    public List<SceneMusicEntry> sceneMusicMapping = new List<SceneMusicEntry>();

    [System.Serializable]
    public class SceneMusicEntry
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    private float currentMusicVolume;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null) musicSource = GetComponent<AudioSource>();

    }

    void Start()
    {
        LoadVolume();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        foreach (var entry in sceneMusicMapping)
        {
            if (entry.sceneName == sceneName)
            {
                clipToPlay = entry.musicClip;
                break;
            }
        }

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
        else if (clipToPlay == null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else if (clipToPlay != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            mainMixer.SetFloat("MusicVolume", -80f);
        }
        else
        {
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
        currentMusicVolume = volume;
        SaveVolume(volume);
    }

    public float GetMusicVolume()
    {
        float volumeInDb;
        mainMixer.GetFloat("MusicVolume", out volumeInDb);

        if (volumeInDb <= -79f)
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
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
            SetMusicVolume(savedVolume);
        }
        else
        {
            SetMusicVolume(0.0f); // Volume padrão
        }
    }
}