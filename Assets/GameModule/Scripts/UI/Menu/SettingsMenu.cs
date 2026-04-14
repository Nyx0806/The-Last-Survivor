using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Controls")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider sensSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        // Load Sound Settings
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        if (bgmSlider != null) bgmSlider.value = savedBGM;
        if (sfxSlider != null) sfxSlider.value = savedSFX;
        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);

        // Load Fullscreen Setting
        int savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1); 
        bool isFullscreen = (savedFullscreen == 1);
        if (fullscreenToggle != null) 
            fullscreenToggle.isOn = isFullscreen;
        SetFullscreen(isFullscreen);

        // Load Mouse Sensitivity
        float savedSens = PlayerPrefs.GetFloat("MouseSens", 1.0f);
        if (sensSlider != null) sensSlider.value = savedSens;
        SetMouseSensitivity(savedSens);
    }

    public void SetBGMVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.bgmSource.volume = volume;
            AudioManager.Instance.bgmVolume = volume; 
        }
        PlayerPrefs.SetFloat("BGMVolume", volume); 
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.sfxSource.volume = volume;
            AudioManager.Instance.sfxVolume = volume; 
        }
        PlayerPrefs.SetFloat("SFXVolume", volume); 
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetMouseSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSens", sens);

        ThirdPersonController player = FindAnyObjectByType<ThirdPersonController>();
        
        if (player != null)
        {
            player.mouseSensitivity = sens;
        }
    }
}