using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuFlicker : MonoBehaviour
{
    private Image bgImage;
    public float minAlpha = 0.4f;
    public float maxAlpha = 1.0f;

    void Start() {
        bgImage = GetComponent<Image>();
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine() {
        while (true) {
            // Tạo độ trễ ngẫu nhiên để nhấp nháy không bị đều quá
            float waitTime = Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(waitTime);
            
            // Thay đổi độ trong suốt ngẫu nhiên
            Color c = bgImage.color;
            c.a = Random.Range(minAlpha, maxAlpha);
            bgImage.color = c;
        }
    }
}