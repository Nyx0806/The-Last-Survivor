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
    public float minimumLoadTime = 3f;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!operation.isDone)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

            float displayProgress = Mathf.Min(realProgress, timeProgress);

            if (progressBarFill != null) progressBarFill.fillAmount = displayProgress;

            if (elapsedTime >= minimumLoadTime && operation.progress >= 0.9f)
            {
                if (progressBarFill != null) progressBarFill.fillAmount = 1f;
                
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}