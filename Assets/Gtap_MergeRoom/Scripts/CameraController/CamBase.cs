using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public abstract class CamBase : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCam;
    private float _shakerTimer;
    private CinemachineBasicMultiChannelPerlin _camBasicMultiChannelPerlin;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    
    private void Awake()
    {
        _virtualCam = GetComponent<CinemachineVirtualCamera>();
        _camBasicMultiChannelPerlin = _virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    public void SetMain()
    {
        _virtualCam.Priority = 1;
    }

    public void Hide()
    {
        _virtualCam.Priority = 0;
    }

    public void SetFollow(Transform follow, Transform aim = null)
    {
        _virtualCam.Follow = follow;
        _virtualCam.LookAt = aim;
    }
    
    public void ShakeCamera(float intensity, float duration)
    {
        _camBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        _shakerTimer = duration;
    }

    private void Update()
    {
        if(_shakerTimer > 0f)
        {
            _shakerTimer -= Time.deltaTime;
            if (_shakerTimer <= 0f)
            {
                _camBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}