using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(GDPRController))]
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private SplashScreenController _splashScreen;

    private CoreSettings _coreSettings;
    private GDPRController _gdprController;

    private async void Awake()
    {
        _coreSettings = Resources.Load("CoreSettings") as CoreSettings;

        _splashScreen.ScreenActive = _coreSettings.IsCoreStart;
        _gdprController = GetComponent<GDPRController>();
        
        Application.targetFrameRate = _coreSettings.TargetFrameRateValue;

        _gdprController.Setup();

        if(_coreSettings.EnableRemoteAdsService)
        {
            while (_gdprController.IsShow)
            {
                await Task.Yield();
            }

            RemoteConfigController.Instance.Setup();

            while (RemoteConfigController.Instance.Fetched == false)
            {
                await Task.Yield();
            }

            AdsController.Instance.Setup(RemoteConfigController.Instance.Value);

            while (AdsController.Instance.IsInitialization == false)
            {
                await Task.Yield();
            }
        }

        if(_coreSettings.FpsCounterEnabled)
            Instantiate(_coreSettings.FpsCounter);

        if (_coreSettings.Scenes.Count == 0)
        {
            Debug.LogError("Scenes are not added to the build");
        }
        
        SceneController.Instance.Setup(_coreSettings.Scenes, _splashScreen, _coreSettings.CoreScene);
        
        await Task.Delay(100);
        
        if(_coreSettings.IsCoreStart)
        {
            SceneController.Instance.LoadingScene();
        }
    }

    private void Start()
    {
        Destroy(this.gameObject);
    }
}