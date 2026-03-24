using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinPanelUI : MonoBehaviour
{
    public Button restartButton;
    public Button quitButton;

    IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.RegisterWinPanel(gameObject);

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(GameManager.Instance.Restart);
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(GameManager.Instance.Quit);

        gameObject.SetActive(false);
    }
}
