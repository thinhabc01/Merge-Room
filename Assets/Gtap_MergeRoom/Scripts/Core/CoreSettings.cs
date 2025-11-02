using System.Collections.Generic;
#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "CoreSettings", menuName = "Data/CoreSettings")]
public class CoreSettings : ScriptableObject
{
    private enum TargetFrameRate
    {
        Frame_Rate_30,
        Frame_Rate_60,
    }

    private const string _coreScene = "[Gtap]CoreScene";
    
    [Header("Enable remote-ads service"), Space(10)] 
    [SerializeField] private bool _enableRemoteAdsService = true;
    
    [Header("Target frame rate"), Space(10)]
    [SerializeField] private TargetFrameRate _frameRate;

    [Header("Debug settings"), Space(10)]
    [SerializeField] private bool _fpsCounterEnabled;
    [SerializeField] private GameObject _fpsCounter;

    [Header("Scenes by build"), Space(10)]
    [SerializeField] private List<string> _scenes;
    
    public int TargetFrameRateValue => _frameRate == TargetFrameRate.Frame_Rate_30 ? 30 : 60;
    public bool FpsCounterEnabled => _fpsCounterEnabled;
    public GameObject FpsCounter => _fpsCounter;
    public List<string> Scenes => _scenes;
    public string CoreScene => _coreScene;
    public bool IsCoreStart { get; set; }
    
    public bool EnableRemoteAdsService => _enableRemoteAdsService;

#if UNITY_EDITOR
    [Button]
    private void GetScenesBuild()
    {
        _scenes.Clear();
        
        _scenes = EditorBuildSettings.scenes
            .Where( scene => scene.enabled )
            .Select( scene => scene.path.Substring(0, scene.path.Length - 6).Substring(scene.path.LastIndexOf('/') + 1))
            .ToList();

        for (int i = 0; i < _scenes.Count; i++)
        {
            if (_scenes[i] == _coreScene)
            {
                _scenes.RemoveAt(i);
                break;
            }
        }
    } 
#endif
}