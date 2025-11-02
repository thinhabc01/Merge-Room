using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _main;

    public void Awake()
    { 
        _particleSystem = GetComponent<ParticleSystem>();
        _main = _particleSystem.main;
        _main.stopAction = ParticleSystemStopAction.Callback;
    }

    public void OnEnable()
    { 
        _particleSystem.Play();
    }

    private void OnParticleSystemStopped()
    {
        PoolManager.SetPool(this.gameObject);
    }
}