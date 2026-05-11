using System.Collections;
using UnityEngine;

public class OpenChest : MonoBehaviour
{
    [SerializeField] private float openingSpeed;
    private Quaternion _closedRotation;
    private Coroutine _currentCoroutine;

    private bool _isOpen;
    private Quaternion _openRotation;

    private void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = _closedRotation * Quaternion.Euler(-90, 00, 0);
    }

    private void Open()
    {
        if (TryGetComponent(out PlaySFX sfx)) sfx.Play();
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(Opening());
    }

    private IEnumerator Opening()
    {
        var targetRotation = _isOpen ? _closedRotation : _openRotation;
        _isOpen = !_isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                openingSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRotation;
    }
    
    private void OnEnable()
    {
        LockControl.UnLocked += Open;
    }

    private void OnDisable()
    {
        LockControl.UnLocked -= Open;
    }
}