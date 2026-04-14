using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SimplePanelFade : MonoBehaviour
{
    public float fadeSpeed = 2f; // Tốc độ hiện ra
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        canvasGroup.alpha = 0; 
        transform.localScale = Vector3.one * 0.9f;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.unscaledDeltaTime * fadeSpeed * 0.2f);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1;
        transform.localScale = Vector3.one;
    }
}