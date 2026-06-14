using System;
using System.Collections;
using UnityEngine;

public class RotateWheel : MonoBehaviour, IUsable
{
    public static event Action<string, int> Rotated = delegate { };

    private bool _coroutineAllowed;
    private int _numberShown;
    
    private void Start()
    {
        _coroutineAllowed = true;
        _numberShown = 0; 
    }
    
    private void OnEnable()
    {
        LockControl.UnLocked += DisableWheel;
    }

    private void OnDisable()
    {
        LockControl.UnLocked -= DisableWheel;
    }

    private void DisableWheel()
    {
        gameObject.SetActive(false);
    }
    
    public bool Use(RaycastHit? hit)
    {
        if (!_coroutineAllowed) return false;
        StartCoroutine(RotateWheelCoroutine());
        return false;
    }

    private IEnumerator RotateWheelCoroutine()
    {
        _coroutineAllowed = false;

        for (int i = 0; i <= 11; i++)
        {
            transform.Rotate(-3.0f, 0f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        _coroutineAllowed = true;

        _numberShown += 1;

        if (_numberShown > 9)
        {
            _numberShown = 0;
        }

        Rotated(name, _numberShown);
    }

}