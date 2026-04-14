using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    public float bgmVolume; 
    public float sfxVolume;

    [Header("Background Music")]
    public AudioClip menuBGM;
    public AudioClip gameplayBGM;

    [Header("Game State Music")]
    public AudioClip winMusic;
    public AudioClip loseMusic;

    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;

        PlayBGM(menuBGM);
    }

    // --- CÁC HÀM PHÁT NHẠC NỀN ---
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        if (bgmSource.clip == clip) return; 

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // --- CÁC HÀM PHÁT HIỆU ỨNG (SFX) ---
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayClick() => PlaySFX(buttonClick);
    public void PlayHover() => PlaySFX(buttonHover);
    public void PlayWinMusic() 
    {
        bgmSource.Stop();
        PlaySFX(winMusic);
    }
    public void PlayLoseMusic()
    {
        bgmSource.Stop();
        PlaySFX(loseMusic);
    }
}