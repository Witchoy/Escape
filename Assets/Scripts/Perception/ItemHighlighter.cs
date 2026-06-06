using UnityEngine;

public class ItemHighlighter : MonoBehaviour, IHighlightable
{
    [SerializeField] private Material highlightMaterial;
    private Material _originalMaterial;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.material;
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