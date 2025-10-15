using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUICharacterMenuManager : PlayerUIMenu
{
    [Header("Audio Mixer")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider mixerSlider;


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("mixerVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
        }

    }

    public void SetMasterVolume()
    {
        float volume = Mathf.Clamp(mixerSlider.value, 0.0001f, 1f);
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("mixerVolume", volume);
    }

    private void LoadVolume()
    {
        mixerSlider.value = PlayerPrefs.GetFloat("mixerVolume");
        SetMasterVolume();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            LoadVolume();
        }
    }
}
