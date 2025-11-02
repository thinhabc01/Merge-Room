using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private SlicedFilledImage _progressLine;
    [SerializeField] private float _maxSpeedFill = 0.5f;
    [SerializeField] private float _minSpeedFill = 0.03f;

    private float _currentValue;
    private float _initialValue;

    public void ChangeValue(float value, bool smooth)
    {
        _initialValue = _currentValue;
        _currentValue = value;
        
        if(smooth) return;
        
        _progressLine.fillAmount = value;
    }

    private void LateUpdate()
    {
        if (_progressLine.fillAmount != _currentValue)
        {
            var t = Mathf.InverseLerp(_initialValue, _currentValue, _progressLine.fillAmount);
            var step = Time.smoothDeltaTime * Mathf.Lerp(_maxSpeedFill, _minSpeedFill, t); 
            
            _progressLine.fillAmount =
                Mathf.MoveTowards(_progressLine.fillAmount, _currentValue,  step);
        }
    }
}
