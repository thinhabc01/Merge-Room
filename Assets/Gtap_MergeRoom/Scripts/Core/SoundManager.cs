using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private AudioSource _musicAudioSource;
    
    private LinkedList<AudioSource> _poolAudioSources = new LinkedList<AudioSource>();
    private SoundManagerComponent _component;
    private AudioSource _audioSourceMusic;

    public bool IsSounds
    {
        get => !_component.Mute;
        set => _component.Mute = !value;
    }

    public bool IsMusic
    {
        set => _audioSourceMusic.mute = !value;
    }
    
    public void Setup(Dictionary<string, bool> settings)
    {
        ClearPool();
        
        var obj = new GameObject(GetType().Name);
        _component = obj.AddComponent<SoundManagerComponent>();
        
        _audioSourceMusic ??= this.gameObject.AddComponent<AudioSource>();

        IsSounds = settings["OnSounds"];
        IsMusic = settings["OnMusic"];
    }
    
    public AudioSource PlaySound(AudioClip clip, bool isLoop = false, float volume = 1f)
    {
        if (IsSounds)
        {
            var audioSource = Instance.GetSound();

            audioSource.clip = clip;
            audioSource.loop = isLoop;
            audioSource.volume = volume;
            audioSource.Play();

            return audioSource;
        }

        return null;
    }

    public void PlayMusic(AudioClip clip, bool isLoop = true, float volume = 1f)
    {
        _audioSourceMusic.clip = clip;
        _audioSourceMusic.loop = isLoop;
        _audioSourceMusic.volume = volume;
        _audioSourceMusic.Play();
    }
    
    public void StopMusic()
    {
        _audioSourceMusic.Stop();
    }
    
    private AudioSource GetSound()
    {
        AudioSource result;
        
        if (_poolAudioSources.Count > 0)
        {
            result = _poolAudioSources.First.Value;
            result.gameObject.SetActive(true);
            _component.AddAudioSources(result);
            
            _poolAudioSources.RemoveFirst();
            
            return result;
        }

        var obj = new GameObject("SoundObject");
        result = obj.AddComponent<AudioSource>();
        result.playOnAwake = false;
        
        _component.AddAudioSources(result);

        return result;
    }

    public void PutPoolSound(AudioSource audioSource)
    {
        Instance._poolAudioSources.AddFirst(audioSource);
        
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);
    }

    private void ClearPool()
    {
        Instance._poolAudioSources.Clear();
    }	
}