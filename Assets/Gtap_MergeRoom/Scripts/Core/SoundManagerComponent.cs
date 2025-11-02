using UnityEngine;
using System.Collections.Generic;

public class SoundManagerComponent : MonoBehaviour
{
    private List<AudioSource> _activeAudioSources = new List<AudioSource>();

    private bool _mute;
    
    public bool Mute
    {
        get => _mute;
        set
        {
            _mute = value;
            foreach (var current in _activeAudioSources)
            {
                current.mute = _mute;
            }
        }
    }
    
    public void Update()
    {
        UpdateList();
    }

    public void AddAudioSources(AudioSource audioSource)
    {
        _activeAudioSources.Add(audioSource);
        audioSource.transform.parent = this.transform;
    }

    private void UpdateList()
    {
        for (int i = 0; i < _activeAudioSources.Count; i++)
        {
            if (_activeAudioSources[i].isPlaying == false)
            {
                SoundManager.Instance.PutPoolSound(_activeAudioSources[i]);
                _activeAudioSources.RemoveAt(i);
                i--;
            }
        }
    }
}
