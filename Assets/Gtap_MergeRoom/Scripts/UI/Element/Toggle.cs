using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIGame
{
    public class Toggle : MonoBehaviour
    {
        [Serializable]
        public struct Colors
        {
            public Color Handle;
            public Color Background;
        }

        public event Action<bool> StateChanged;

        [Header("Colors")] [SerializeField] private Colors _enabledColors;
        [SerializeField] private Colors _disabledColors;

        [Space, Header("Do not change")] [SerializeField]
        private Button _button;

        [SerializeField] private Image _handleImage;
        [SerializeField] private Image _backgroundImage;


        private RectTransform _rect;
        private Vector2 _handlePosition;
        private bool _currentState;

        public void Setup(bool initialState)
        {
            _currentState = initialState;

            _button.onClick.AddListener(OnButtonClick);

            _rect = _handleImage.rectTransform;

            ToggleState();
        }

        private void OnButtonClick()
        {
            if (!_rect) return;

            _currentState = !_currentState;

            ToggleState();
        }

        private void ToggleState()
        {
            StateChanged?.Invoke(_currentState);

            if (_currentState == false)
            {
                _handleImage.color = _disabledColors.Handle;
                _backgroundImage.color = _disabledColors.Background;
                _handlePosition.x = -25f;
            }
            else
            {
                _handleImage.color = _enabledColors.Handle;
                _backgroundImage.color = _enabledColors.Background;
                _handlePosition.x = 25f;
            }

            _rect.anchoredPosition = _handlePosition;
        }
    }
}