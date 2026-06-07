using System;
using System.Collections;
using UnityEngine;

public class RotateWheel : MonoBehaviour, IHighlightable, IUsable
{
    [SerializeField] private Material highlightMaterial;
    public static event Action<string, int> Rotated = delegate { };

    private bool _coroutineAllowed;
    private int _numberShown;
    private Material _originalMaterial;
    private Renderer _renderer;
    
    private void Start()
    {
        _coroutineAllowed = true;
        _numberShown = 0; 
        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.material;
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
    
    public void Use()
    {
        if (!_coroutineAllowed) return;
        StartCoroutine(RotateWheelCoroutine());
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

    public void Highlight()
    {
        _renderer.material = highlightMaterial;
    }

    public void Unhighlight()
    {
        _renderer.material = _originalMaterial;
    }


}