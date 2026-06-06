using UnityEngine;

public class OpeningKey : MonoBehaviour, IUsable
{
    public bool Use(RaycastHit? hit)
    {
        if (hit == null) return false;
        if (!hit.Value.collider.TryGetComponent(out RotateDoor door)) return false;

        door.Interact();
        return true;
    }
}