using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : UIWindow
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private AudioClip _audioClip;

    private UIManager _uiManager;

    public override void Setup(UIManager uiManager)
    {
        _uiManager = uiManager;

        _closeButton.onClick.AddListener(ContinueGame);

        SetupContent();
    }

    private void SetupContent()
    {
        foreach (var content in GetComponentsInChildren<SettingsContent>())
        {
            content.Setup(content.IsToggle ? _uiManager.GetSettingsValue(content.Key) : true);

            content.OnClick += OnButtonClick;
            content.StateChanged += OnStateChange;
        }
    }

    private void OnButtonClick(string key)
    {
        //TODO: implement on click behaviour;
        return;
    }

    private void OnStateChange(string key, bool value)
    {
        _uiManager.ChangeSetting(key, value);
    }

    private void ContinueGame()
    {
        SoundManager.Instance.PlaySound(_audioClip);

        _uiManager.ShowPreviousWindow();
    }

    protected override void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(ContinueGame);

        foreach (var content in GetComponentsInChildren<SettingsContent>())
        {
            content.OnClick -= OnButtonClick;
            content.StateChanged -= OnStateChange;
        }
    }
}