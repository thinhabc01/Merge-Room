using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private float _durationShow = 0.6f;
    [SerializeField] private float _durationWorld = 1f;
    [SerializeField] private float _scale = 0.7f;
    
    private SpriteRenderer _spriteRenderer;
    private float _scaleWorld;

    public EItem EItem { get; private set; }
    public EItem NextItem { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Setup(ItemData data)
    {
        _spriteRenderer.sprite = data.Sprite;
        _scaleWorld = data.Scale;
        NextItem = data.Next; 
        EItem = data.Type;
        _spriteRenderer.transform.localScale = Vector3.zero;

        AnimationShow(false);
    }

    public void AnimationShow(bool worldScale)
    {
        if(worldScale == false && _spriteRenderer.transform.localScale == Vector3.one * _scale) return;
        
        var scale = worldScale ? _scaleWorld : _scale;
        
        _spriteRenderer.transform.DOKill();
        
        _spriteRenderer.transform
            .DOScale(scale * Vector3.one, worldScale ? _durationWorld : _durationShow)
            .SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        _spriteRenderer.transform.DOKill();
    }
}