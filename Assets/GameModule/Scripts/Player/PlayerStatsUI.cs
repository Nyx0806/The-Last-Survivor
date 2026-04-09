using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerStats playerStats;

    public Image hpBar;
    public Image hungerBar;
    public Image thirstBar;
    [Header ("Damaged Effects")]
    public Image bloodScreen;
    public float fadeSpeed = 2f;
    private Coroutine fadeCoroutine;

    void Update()
    {
        hpBar.fillAmount = playerStats.currentHP / playerStats.statsData.maxHP;

        hungerBar.fillAmount = playerStats.hunger / playerStats.statsData.maxHunger;

        thirstBar.fillAmount = playerStats.thirst / playerStats.statsData.maxThirst;
    }

    public void ShowDamageFlash()
    {
        if (bloodScreen == null) return;
        
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeOutBlood());
    }

    private IEnumerator FadeOutBlood()
    {
        // Bật rõ viền máu
        Color c = bloodScreen.color;
        c.a = 0.5f; 
        bloodScreen.color = c;

        // Trừ dần Alpha về 0 theo thời gian
        while (bloodScreen.color.a > 0)
        {
            c.a -= fadeSpeed * Time.deltaTime;
            bloodScreen.color = c;
            yield return null;
        }
    }

}