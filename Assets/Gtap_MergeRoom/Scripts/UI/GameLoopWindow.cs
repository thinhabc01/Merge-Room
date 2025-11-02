using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLoopWindow: UIWindow
{
    [SerializeField] private TMP_Text _counterTMP;
    [SerializeField] private Button _backToLobby;
    [SerializeField] private Button _buttonOpenSettings;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private ProgressBar _levelProgressBar;

    private UIManager _uiManager;

    public override void Setup(UIManager uiManager)
    {
        _uiManager = uiManager;

        _buttonOpenSettings.onClick.AddListener(ClickButtonSettings);
        _backToLobby.onClick.AddListener(_uiManager.BackToLobby);
    }

    private void ClickButtonSettings()
    {
        SoundManager.Instance.PlaySound(_audioClip);
        
        _uiManager.OpenSettings();
    }

    public void SetValueProgress(float value, int count, int total)
    {
        _levelProgressBar.ChangeValue(value, true);
        
        _counterTMP.SetText($"{count}/{total}");
        _counterTMP.transform.DOKill();
        _counterTMP.transform.localScale = Vector3.one;
        _counterTMP.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1);
    }

    protected override void OnDestroy()
    {
        _counterTMP.transform.DOKill();
        
        _buttonOpenSettings.onClick.RemoveAllListeners();
        _backToLobby.onClick.RemoveAllListeners();
    }
}