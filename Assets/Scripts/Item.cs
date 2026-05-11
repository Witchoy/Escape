using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemSo item;
    public int amount;

    public void Interact()
    {
        InventoryManager.Instance.AddItem(item, amount);
        Destroy(gameObject);
    }
}
