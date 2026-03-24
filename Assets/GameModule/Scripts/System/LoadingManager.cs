using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public Image progressBarFill; 
    public static string SceneToLoad = "Gameplay"; 

    [Header("Settings")]
    public float minimumLoadTime = 3f; // Ép người chơi phải xem màn hình loading ít nhất 3 giây

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        // 1. Bắt đầu tải Scene trong nền
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        
        // 2. NGĂN Unity tự động chuyển cảnh khi tải xong
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!operation.isDone)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // Tiến độ thực tế của Unity (nó sẽ dừng ở 0.9 nếu allowSceneActivation = false)
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Tiến độ giả (dựa trên thời gian tối thiểu bạn muốn)
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

            // 3. Thanh Loading sẽ chạy theo cái nào chậm hơn (để mượt mà và không bao giờ chạy lố)
            float displayProgress = Mathf.Min(realProgress, timeProgress);

            // Cập nhật UI
            if (progressBarFill != null) progressBarFill.fillAmount = displayProgress;

            // 4. Nếu đã đủ thời gian chờ VÀ Unity đã nạp xong dữ liệu ở nền
            if (elapsedTime >= minimumLoadTime && operation.progress >= 0.9f)
            {
                // Ép UI hiển thị tròn 100% trước khi chuyển
                if (progressBarFill != null) progressBarFill.fillAmount = 1f;
                
                // Cho phép Unity nổ máy chuyển cảnh!
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}