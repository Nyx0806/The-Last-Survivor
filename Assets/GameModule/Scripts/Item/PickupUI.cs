using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text text;

    public void Show(string itemName)
    {
        panel.SetActive(true);
        text.text = "Press F to pick up " + itemName;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}