using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusReset : MonoBehaviour
{
    void OnEnable()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
