// Attach to: Player GameObject (same as HotBarController)
using UnityEngine;
using UnityEngine.UI;

public class HandEquipper : MonoBehaviour
{
    private const float EquippedOpacity = 0.9f;
    private const float NormalOpacity = 0.5f;

    [SerializeField] private Transform hand;

    private HotBarController _hotBarController;
    private GameObject _currentHandItem;

    private void Awake()
    {
        _hotBarController = GetComponent<HotBarController>();
    }

    private void OnEnable()
    {
        _hotBarController.OnEquippedChanged += Refresh;
    }

    private void OnDisable()
    {
        _hotBarController.OnEquippedChanged -= Refresh;
    }

    private void Start() => Refresh();

    private void Refresh()
    {
        RefreshHandItem();
        RefreshOpacity();
    }

    private void RefreshHandItem()
    {
        if (_currentHandItem) Destroy(_currentHandItem);

        var slots = InventoryManager.Instance.HotBarSlots;
        var slot = slots[_hotBarController.EquippedIndex];
        if (!slot.HasItem()) return;

        var item = slot.GetItem();
        if (!item.handPrefab) return;

        _currentHandItem = Instantiate(item.handPrefab, hand);
        _currentHandItem.transform.localPosition = item.handPositionOffset;
        _currentHandItem.transform.localRotation = Quaternion.Euler(item.handRotationOffset);
    }

    private void RefreshOpacity()
    {
        var slots = InventoryManager.Instance.HotBarSlots;
        for (var i = 0; i < slots.Count; i++)
        {
            var icon = slots[i].GetComponent<Image>();
            if (!icon) continue;
            icon.color = i == _hotBarController.EquippedIndex
                ? new Color(1, 1, 1, EquippedOpacity)
                : new Color(1, 1, 1, NormalOpacity);
        }
    }

    public IUsable GetEquippedUsable() =>
        _currentHandItem ? _currentHandItem.GetComponent<IUsable>() : null;

    public Transform GetEquippedItemTransform() =>
        _currentHandItem ? _currentHandItem.transform : null;
}
