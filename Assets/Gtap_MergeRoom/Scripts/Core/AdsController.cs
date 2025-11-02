using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AdsController : Singleton<AdsController>
{
    private bool _adsShow;
    private string _ironSourceAndroidId;
    private float _cooldownAdsShow;
    public float CurrentTime { get; private set; }
    
    public bool IsInitialization { get; private set; }

    public void Setup(RemoteConfigController.Settings settings)
    {
        _adsShow = settings.AdsShow;
        _cooldownAdsShow = settings.CooldownAdsShow;
        _ironSourceAndroidId = settings.IronSourceAndroidId;

        Initialize();
    }

    private void Initialize()
    {
        UniTask.Void(async () =>
        {
            try
            { 
                await UniTask.Delay(100); 
                IronSource.Agent.init(_ironSourceAndroidId);
                IronSource.UNITY_PLUGIN_VERSION = "7.2.1-ri";
                IsInitialization = true;
                
                if(_adsShow)
                {
                    IronSource.Agent.loadInterstitial();
                    CurrentTime = _cooldownAdsShow;
                    
                    Subscribe();
                }
            }
            catch (Exception e) 
            { 
                Debug.LogException(e);
            }
        });
    }

    private void Subscribe()
    {
        //IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        //IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        //IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        //IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        //IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        //IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        //IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        //IronSourceEvents.onRewardedVideoAdClosedEvent += CheckTimerInterstitialAdsAfterRewardAds;
    }

    private void Update()
    {
        if (_adsShow == false) return;

        CurrentTime -= Time.deltaTime;
    }

    public void ShowInterstitialAds()
    {
        if (CurrentTime < 0f)
        {
            var available = IronSource.Agent.isInterstitialReady();
            if (available)
            {
                CurrentTime = _cooldownAdsShow;

                IronSource.Agent.showInterstitial();
            }
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void CheckTimerInterstitialAdsAfterRewardAds()
    {
        /*if (CurrentTime < 10f)
        {
            CurrentTime += 10f;
        }*/
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (_adsShow == false) return;

        //IronSourceEvents.onInterstitialAdReadyEvent -= InterstitialAdReadyEvent;
        //IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;
        //IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent;
        //IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent;
        //IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
        //IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
        //IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;
        //IronSourceEvents.onRewardedVideoAdClosedEvent -= CheckTimerInterstitialAdsAfterRewardAds;
    }

    #region EventsAds

    private void InterstitialAdReadyEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdReadyEvent");
    }

    private void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        IronSource.Agent.loadInterstitial();
    }

    private void InterstitialAdShowSucceededEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
    }

    private void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        IronSource.Agent.loadInterstitial();
    }

    private void InterstitialAdClickedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdClickedEvent");
    }

    private void InterstitialAdOpenedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
    }

    private void InterstitialAdClosedEvent()
    {
        IronSource.Agent.loadInterstitial();
    }

    #endregion
}