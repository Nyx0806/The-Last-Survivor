using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject opt_panel;

    public void PlayGame()
    {
        LoadingManager.SceneToLoad = "Gameplay"; 
        SceneManager.LoadScene("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (opt_panel != null)
            opt_panel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (opt_panel != null)
            opt_panel.SetActive(false);
    }
}
