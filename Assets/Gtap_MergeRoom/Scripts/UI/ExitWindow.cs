using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ExitWindow : UIWindow
{
    [SerializeField] private Button _buttonExitGame;
    [SerializeField] private Button _buttonContinueGame;

    [Space, Header("Continue Button Animation")] 
    [SerializeField] private float _endSize = 1.1f;
    [SerializeField] private float _animationTime = 0.7f;
    
    private UIManager _uiManager;
    
    public override void Setup(UIManager uiManager)
    {
        _uiManager = uiManager;
        
        _buttonExitGame.onClick.AddListener(ExitApplication);
        _buttonContinueGame.onClick.AddListener(ContinueGame);

        OnShowing += OnShow;
    }

    private void ExitApplication()
    {
        _buttonContinueGame.transform.DOKill();
        
        _buttonContinueGame.interactable = false;
        _buttonExitGame.interactable = false;
        
        Application.Quit();
    }

    private void OnShow()
    {
        _buttonContinueGame.transform.DOScale(_endSize, _animationTime)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    private void ContinueGame()
    {
        _buttonContinueGame.transform.DOKill();
        
        _uiManager.ShowPreviousWindow();
    }
    
    protected override void OnDestroy()
    {
        OnShowing -= OnShow;
        
        _buttonContinueGame.transform.DOKill();
        _buttonExitGame.onClick.RemoveListener(ExitApplication);
        _buttonContinueGame.onClick.RemoveListener(ContinueGame);
    }
}
