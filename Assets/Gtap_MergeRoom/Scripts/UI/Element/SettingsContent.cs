using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UIGame.Toggle;

public class SettingsContent : MonoBehaviour
{
    public event Action<string, bool> StateChanged;
    public event Action<string> OnClick;

    [SerializeField] private string _key;
    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private string _name;

    [Space, Header("Do not change")] 
    [SerializeField] private Button _button;

    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _title;

    public string Key => _key;
    public bool IsToggle => _toggle != null;

    public void Setup(bool initialState)
    {
        _icon.sprite = _iconSprite;
        _title.text = _name;
        
        if (IsToggle)
        {
            _toggle.StateChanged += OnStateChange;
            _toggle.Setup(initialState);
        }
        else if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            throw new Exception("No button assigned");
        }
    }

    private void OnButtonClick() => OnClick?.Invoke(_key);

    private void OnStateChange(bool value)
    {
        StateChanged?.Invoke(_key, value);
        ES3.Save(Key, value);
    }
}