using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject winPanel;
    private GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void RegisterWinPanel(GameObject panel)
    {
        winPanel = panel;
    }

    public void RegisterGameOverPanel(GameObject panel)
    {
        gameOverPanel = panel;
    }

    // ===== WIN CHECK =====
    public void CheckWinCondition()
    {
        int alive = EnemyPool.Instance.GetAliveEnemyCount();

        if (alive == 0)
            WinGame();
    }

    public void WinGame()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
            
        FindAnyObjectByType<SurvivalTimer>()?.StopTimer();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayWinMusic();
        StopPlayer();
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        FindAnyObjectByType<SurvivalTimer>()?.StopTimer();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLoseMusic();

        StopPlayer();
    }

    void StopPlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        var controller = FindAnyObjectByType<ThirdPersonController>();
        if (controller != null)
            controller.enabled = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        LoadingManager.SceneToLoad = "Gameplay"; 
        SceneManager.LoadScene("Loading");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay")
        {
            Time.timeScale = 1f;
            ResetPool();
            ResetRuntimeState();

            SurvivalTimer timer = FindAnyObjectByType<SurvivalTimer>();
            if (timer != null)
            {
                timer.gameObject.SetActive(true);
                timer.ResetTimer();
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayBGM(AudioManager.Instance.gameplayBGM);
        }
    }

    void ResetRuntimeState()
    {
        winPanel = null;
        gameOverPanel = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ResetPool()
    {
        if (EnemyPool.Instance != null)
            EnemyPool.Instance.ResetPool();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}