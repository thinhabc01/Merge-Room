using UnityEngine;

public class GDPRController : MonoBehaviour
{
    [SerializeField] private bool _enabled;
    [SerializeField] private GameObject _windowGdpr;
    
    private const string _firstShowGdprkey = "FirstShowGDPR";
    
    public bool IsShow { get; private set; }

    public void Setup()
    {
        if(!_enabled) return;
        
        var firstShow = ES3.Load(_firstShowGdprkey, false);
        
        if(firstShow == false)
        {
            IsShow = true;
            //MaxSdkCallbacks.OnSdkInitializedEvent += ShowGdpr;
        }
    }
    
    /*private void ShowGdpr(MaxSdkBase.SdkConfiguration sdkConfiguration)
    {
        if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies)
        { 
            _windowGdpr.gameObject.SetActive(true);
            return;
        }

        if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.DoesNotApply)
        {
            IsShow = false;
            return;
        }
        
        _windowGdpr.gameObject.SetActive(true);
    }

    public void OnClickButtonAccept()
    {
        MaxSdk.SetHasUserConsent(true);
        ES3.Save(_firstShowGdprkey, true);

        _windowGdpr.SetActive(false);
        IsShow = false;
    }

    public void OpenUrl()
    {
        Application.OpenURL("https://devgame.me/policy");
    }

    private void OnDestroy()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent -= ShowGdpr;
    }*/
}