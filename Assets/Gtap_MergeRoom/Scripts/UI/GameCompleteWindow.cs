using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCompleteWindow : UIWindow
{
    [SerializeField] private Button _buttonNextLevel;
    [SerializeField] private TMP_Text _levelTMP;
    [SerializeField] private CanvasGroup _groupButtonContinue;
    [SerializeField] private Image _background;
    [SerializeField] private Transform _roomIcon;
    [SerializeField] private Image _roomImage;
    [SerializeField] private AudioClip _winClip;

    private UIManager _uiManager;
    private Sequence _sequence;

    public Sprite CurrentRoomSprite
    {
        set => _roomImage.sprite = value;
    }

    public override void Setup(UIManager uiManager)
    {
        _uiManager = uiManager;
        
        _buttonNextLevel.onClick.AddListener(ClickButtonNextLevel);

        OnShowing += AnimationWindow;
    }

    private void ClickButtonNextLevel()
    {
        _buttonNextLevel.transform.DOKill();
        _buttonNextLevel.interactable = false;
        _uiManager.BackToLobby();
    }

    private void AnimationWindow()
    {
        _buttonNextLevel.interactable = false;
        _buttonNextLevel.transform.localScale = Vector3.one;
        _groupButtonContinue.alpha = 0f;
        _levelTMP.alpha = 0f;
        _background.SetAlpha(0f);
        _roomIcon.localScale = Vector3.zero;

        _sequence = DOTween.Sequence();
        _sequence
            .Append(_levelTMP
                    .DOFade(1f, 1.5f)
                    .SetDelay(0.5f)
                    .SetEase(Ease.InQuad))
            .Join(_background
                    .DOFade(1f, 1.5f))
            .Append(_roomIcon
                    .DOScale(1f, 0.6f)
                    .SetEase(Ease.OutBack))
            .AppendCallback(ButtonContinueEnable);
    }

    private void ButtonContinueEnable()
    {
        SoundManager.Instance.PlaySound(_winClip);
        _buttonNextLevel.interactable = true;
        
        _groupButtonContinue.DOFade(1f, 0.3f).OnComplete(() =>
        {
            _buttonNextLevel.transform.DOScale(1.1f, 0.7f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }

    private void Kill()
    {
        _sequence.Kill();
        _groupButtonContinue.DOKill();
        _buttonNextLevel.transform.DOKill();
    }

    private void OnDisable() => Kill();

    protected override void OnDestroy()
    {
        Kill();
        
        _buttonNextLevel.onClick.RemoveAllListeners();
        OnShowing -= AnimationWindow;
    }
}
