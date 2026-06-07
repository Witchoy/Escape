using UnityEngine;

public class OpeningKey : MonoBehaviour
{

    public void Use(RaycastHit? hit)
    {
        if (hit == null) return;
        if (!hit.Value.collider.TryGetComponent(out RotateDoor door)) return;

        door.Use();
        
        Destroy(gameObject);
    }
}