using UnityEngine;
using UnityEngine.UI;

public class SurvivalTimer : MonoBehaviour
{
    public float surviveTime = 120f;
    public TMPro.TextMeshProUGUI timerText;

    private float currentTime;
    private bool running;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (!running) return;

        currentTime -= Time.deltaTime;

        UpdateUI();

        if (timerText != null && currentTime <= 10f)
        {
            timerText.color = Color.red;
        }

        if (currentTime <= 0)
        {
            running = false;
            GameManager.Instance.WinGame();
        }
    }

    public void ResetTimer()
    {
        currentTime = surviveTime;
        running = true;
        UpdateUI();
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void StopTimer()
    {
        running = false;

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }
}