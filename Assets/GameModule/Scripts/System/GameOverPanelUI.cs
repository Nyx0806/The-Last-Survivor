using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverPanelUI : MonoBehaviour
{
    public Button restartButton;
    public Button quitButton;

    IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.RegisterGameOverPanel(gameObject);

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(GameManager.Instance.Restart);
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(GameManager.Instance.Quit);

        gameObject.SetActive(false);
    }
}
