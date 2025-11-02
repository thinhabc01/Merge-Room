using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FingerController : MonoBehaviour
{
    [Header("Enable-Disable 'Z' key")]
    
    [SerializeField] private float _defaultAngle = 0f;
    [SerializeField] private Color _defaultColor = Color.white;

    [SerializeField] private float _speedAnimation = 15f;
    [SerializeField] private float _scalePressed = 0.8f;
    [SerializeField] private Color _colorPressed = Color.gray;
    [SerializeField] private float _anglePressed = 18f;
    
    private Image _image;
    private PlayerInput _input;
    private RectTransform _rectTransform;
    private bool _isActive;

    private Vector3 _targetScale;
    private Color _targetColor;
    private Vector3 _targetAngle;
    
    private void Awake()
    {
        _input = new PlayerInput();
        _input.Enable();

        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponentInChildren<Image>();

        _isActive = false;
        _image.gameObject.SetActive(_isActive);
        _rectTransform.eulerAngles = new Vector3(0f, 0f, _defaultAngle);
    }

    private void Update()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            _isActive = !_isActive;
            _image.gameObject.SetActive(_isActive);
        }

        Animation();
        
        var point = _input.Player.TouchPosition.ReadValue<Vector2>();
        _rectTransform.position = point;
    }

    private void Animation()
    {
        if (_input.Player.Touch.IsPressed())
        {
            _targetScale = Vector3.one * _scalePressed;
            _targetColor = _colorPressed;
            _targetAngle = new Vector3(0f, 0f, _anglePressed);
        }
        else
        {
            _targetScale = Vector3.one;
            _targetColor = _defaultColor;
            _targetAngle = new Vector3(0f, 0f, _defaultAngle);
        }

        var speed = Time.deltaTime * _speedAnimation;
        _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, _targetScale, speed);
        _image.color = Color.Lerp(_image.color, _targetColor, speed);
        _rectTransform.eulerAngles = Vector3.Lerp(_rectTransform.eulerAngles, _targetAngle, speed);
    }
}
