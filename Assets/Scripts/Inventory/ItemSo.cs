using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemSo : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public int maxStack;
    public GameObject itemPrefab;
    public GameObject handPrefab;

    [Header("Hand Offset")]
    public Vector3 handPositionOffset;
    public Vector3 handRotationOffset;
}