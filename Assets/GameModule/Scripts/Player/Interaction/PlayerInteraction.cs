using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 2f;
    public Camera playerCamera;

    private PickupUI pickupUI;
    private ItemPickup currentItem;

    void Start()
    {
        pickupUI = FindAnyObjectByType<PickupUI>();
    }

    void Update()
    {
        if (currentItem == null) pickupUI.Hide();
        CheckItem();

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickup();
        }

        Debug.DrawRay(playerCamera.transform.position,
              playerCamera.transform.forward * interactDistance,
              Color.red);
    }

    void CheckItem()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            ItemPickup item = hit.collider.GetComponentInParent<ItemPickup>();

            if (item != null)
            {
                currentItem = item;
                pickupUI.Show(item.itemData.itemName);
                return;
            }
        }

        currentItem = null;
        pickupUI.Hide();
    }

    void TryPickup()
    {
        if (currentItem != null)
        {
            PlayerStats player = GetComponent<PlayerStats>();
            currentItem.Pickup(player);
            pickupUI.Hide();
        }
    }
}