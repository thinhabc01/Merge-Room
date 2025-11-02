using UnityEngine;

public interface IWatcher
{
    void SetFollow<T>(Transform follow, Transform aim = null) where T : CamBase;

    void SetMain<T>() where T : CamBase;

    void Shake(float intensity, float duration);
}