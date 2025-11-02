using System;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    protected event Action OnShowing;
    protected event Action OnHide;
    
    public abstract void Setup(UIManager uiManager);

    public void Show()
    {
        gameObject.SetActive(true);
        
        OnShowing?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        
        OnHide?.Invoke();
    }

    protected abstract void OnDestroy();
}