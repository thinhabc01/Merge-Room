using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : UIWindow
{
    public enum StepTutorial
    {
        Step01,
        Step02,
        Step03,
        Complete,
    }
    
    //[SerializeField] private Image _circleJoystick;
    [SerializeField] private Image _stickJoystick;
    [SerializeField] private Image _hand;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private float _durationFade = 0.5f;
    
    private const string _tutorialCompleteKey = "TutorialComplete";

    private StepTutorial _stepTutorial;
    private UIManager _uiManager;
    private Sequence _sequence;

    public bool IsTutorialComplete { get; private set; }
    
    public override void Setup(UIManager uiManager)
    {
        _uiManager = uiManager;

        IsTutorialComplete = ES3.Load(_tutorialCompleteKey, false);
        
        OnShowing += DelayStartTutorial;
    }

    private async void DelayStartTutorial()
    {
        await Task.Delay(1000);
        Step01();
    }
    
    private void Step01()
    {
        _stepTutorial = StepTutorial.Step01;
        
        _sequence = DOTween.Sequence();
        _sequence
            .Append(_message
                .DOFade(1f, _durationFade)
                .SetEase(Ease.InQuad))
            //.Join(_circleJoystick
                //.DOFade(0.88f, _durationFade))
            .Join(_stickJoystick
                .DOFade(0.88f, _durationFade))
            .Join(_hand
                .DOFade(0.88f, _durationFade))
            .AppendCallback(LoopHandDragPull);
        
        _message.SetText($"tap to pull");
    }

    private void Step02()
    {
        _stepTutorial = StepTutorial.Step02;
        
        _message.DOKill();
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence
            .Append(_message
                .DOFade(1f, _durationFade)
                .SetEase(Ease.InQuad))
            //.Join(_circleJoystick
                //.DOFade(0.88f, _durationFade))
            .Join(_stickJoystick
                .DOFade(0.88f, _durationFade))
            .Join(_hand
                .DOFade(0.88f, _durationFade))
            .AppendCallback(LoopHandDragAim);
        
        _message.SetText($"drag to aim");
    }
    
    private void Step03()
    {
        _stepTutorial = StepTutorial.Step03;
        
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence
            .Append(_hand
                .DOFade(0f, _durationFade))
            .Join(_stickJoystick
                .DOFade(0f, _durationFade));
            //.Join(_circleJoystick
                //.DOFade(0f, _durationFade));
        
        _message.SetText($"release");

        _message.DOFade(0.5f, _durationFade).SetLoops(-1, LoopType.Yoyo);
    }

    private void CompleteTutorial()
    {
        _stepTutorial = StepTutorial.Complete;
        
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence
            .Append(_hand
                .DOFade(0f, _durationFade))
            .Join(_stickJoystick
                .DOFade(0f, _durationFade))
            //.Join(_circleJoystick
                //.DOFade(0f, _durationFade))
            .Join(_message
                .DOFade(0f, _durationFade));
        
        ES3.Save(_tutorialCompleteKey, true);
        IsTutorialComplete = true;
        
        //_uiManager.ButtonStart();
    }

    private void LoopHandDragPull()
    {
        var pos = _hand.transform.position;
        pos.y -= 80f;
        
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence
            .Append(_hand.transform
                .DOMove( pos, 1.5f)
                .SetEase(Ease.OutQuad))
            .AppendInterval(1f)
            .SetLoops(-1);
    }
    
    private void LoopHandDragAim()
    {
        var pos = _hand.transform.position;
        pos.x -= 120f;
        pos.y = Screen.height / 2f;
        _hand.transform.position = pos;
        
        pos.x += 240f;
        
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence
            .Append(_hand.transform
                .DOMove( pos, 1.1f))
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        // TODO: CONDITION FOR TRANSITION
        if (_stepTutorial == StepTutorial.Step01)
        {
            Step02();
        }

        // TODO: CONDITION FOR TRANSITION
        if (_stepTutorial == StepTutorial.Step02)
        {
            Step03();
        }

        // TODO: CONDITION FOR TRANSITION
        if (_stepTutorial == StepTutorial.Step03)
        {
            CompleteTutorial();
        }
    }

    protected override void OnDestroy()
    {
        _sequence.Kill();
        _message.DOKill();
        OnShowing -= DelayStartTutorial;
    }
}
