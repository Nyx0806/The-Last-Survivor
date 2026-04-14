using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HorrorFlickerText : MonoBehaviour
{
    public float minDelay = 0.05f;
    public float maxDelay = 0.3f;
    
    private TextMeshProUGUI textComponent;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            Color c = textComponent.color;
            
            c.a = Random.Range(0, 100) > 20 ? 1f : 0.2f; 
            textComponent.color = c;

            yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));
        }
    }
}