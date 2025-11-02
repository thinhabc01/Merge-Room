using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class RemoteConfigController : Singleton<RemoteConfigController>
{
    private readonly float _cooldownAdsShowDefault = 40f;
    private readonly bool _adsShowDefault = false;
    private readonly string _ironSourceAndroidIdDefault = "161d27565";
    
    public Settings Value { get; private set; }
    public bool Fetched { get; private set; }

    public event Action<Settings> OnLoadRemoteSettingsEvent;

    public async void Setup()
    {
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        UpdateConfig();
    }
    
    private async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public void UpdateConfig()
    {
        Fetched = false;
        RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }
    
    private void ApplyRemoteSettings(ConfigResponse configResponse) 
    {
        switch (configResponse.requestOrigin) 
        {
            case ConfigOrigin.Default:
                Debug.Log("<Remote Config> No settings loaded this session; using default values.");
                Value = new Settings(_cooldownAdsShowDefault, _adsShowDefault, _ironSourceAndroidIdDefault);
                break;
            case ConfigOrigin.Cached:
                Debug.Log("<Remote Config> No settings loaded this session; using cached values from a previous session.");
                CopyRemoteToField();
                break;
            case ConfigOrigin.Remote:
                Debug.Log("<Remote Config> New settings loaded this session; update values accordingly.");
                CopyRemoteToField();
                break;
        }

        Fetched = true;
        OnLoadRemoteSettingsEvent?.Invoke(Value);
    }

    private void CopyRemoteToField()
    {
        Value = new Settings()
        {
            CooldownAdsShow = RemoteConfigService.Instance.appConfig.GetFloat("CooldownAdsShow"),
            AdsShow = RemoteConfigService.Instance.appConfig.GetBool("AdsShow"),
            IronSourceAndroidId = RemoteConfigService.Instance.appConfig.GetString("IronSourceAndroidId"),
        };
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
    }

    private struct userAttributes
    {
    }

    private struct appAttributes
    {
    }

    [Serializable]
    public struct Settings
    {
        public float CooldownAdsShow;
        public bool AdsShow;
        public string IronSourceAndroidId;
        
        public Settings(float time, bool adsShow, string ironSourceAndroidId)
        {
            CooldownAdsShow = time;
            AdsShow = adsShow;
            IronSourceAndroidId = ironSourceAndroidId;
        }
    }
}
