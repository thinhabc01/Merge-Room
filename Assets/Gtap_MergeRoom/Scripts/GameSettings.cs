using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private AudioClip _mergeClip;
    [SerializeField] private AudioClip _openObjectClip;
    
    [Space(10)] 
    [SerializeField] private GridCellsSettings _gridCellsSettings;
    [SerializeField] private ParticleController _mergeFX;
    [SerializeField] private Item _itemPrefab;

    public GridCellsSettings GridCellsSettings => _gridCellsSettings;
    public AudioClip MusicClip => _musicClip;
    public AudioClip MergeClip => _mergeClip;
    public AudioClip OpenObjectClip => _openObjectClip;
    public Item Item => _itemPrefab;
    public ParticleController MergeFX => _mergeFX;
    
    private readonly Dictionary<string, bool> _defaultSetting = new Dictionary<string, bool>()
    {
        { "OnSounds", true },
        { "OnMusic", true },
        { "OnVibration", true },
        { "Tutorial", true}
    };

    public Dictionary<string, bool> Values { get; private set; }

    public Dictionary<string, bool> LoadSettings()
    {
        Values = ES3.Load("Settings", _defaultSetting);
        return Values;
    }

    public void SaveSettings()
    {
        ES3.Save("Settings", Values);
    }
}