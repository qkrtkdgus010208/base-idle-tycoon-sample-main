namespace Project.Gameplay
{
	using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// Button Tap & Press
    /// </summary>
    public class HoldTappingButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Delay time applied when the button is pressed,
        /// to prevent the action from being triggered too frequently during continuous press.
        /// </summary>
        private const float DELAY_PRESS_TIME = 0.1f;

        /// <summary>
        /// Image target for button
        /// </summary>
        [SerializeField] private Image _buttonImage;

        /// <summary>
        /// Color transition for each button action (normar, pressed, highlight, disable, etc)
        /// </summary>
        [SerializeField] private ColorBlock _colorBlock;

        /// <summary>
        /// on pressed state
        /// true: pointer down
        /// false: pointer up
        /// </summary>
        private bool isOnPressed;

        /// <summary>
        /// button disable state
        /// </summary>
        private bool isDisable;

        /// <summary>
        /// Button on clicked or pressed action
        /// </summary>
        private Action buttonFunction;

        /// <summary>
        /// Pressed delay counter on update
        /// </summary>
        private float pressDelayTimer;


        private void Awake()
        {
            InputListener.SetInteruptInputListener += OnInteruptInputListenerStateChanged;
        }

        private void Update()
        {
            if (!isOnPressed || isDisable) return;

            pressDelayTimer += Time.deltaTime; // delay counter
            if (pressDelayTimer > DELAY_PRESS_TIME) // when delaytimer exceed the delay time
            {
                buttonFunction?.Invoke(); // trigger button function
                pressDelayTimer = 0f; // reset delay counter
            }
        }

        private void OnDestroy()
        {
            InputListener.SetInteruptInputListener -= OnInteruptInputListenerStateChanged;
        }

        /// <summary>
        /// Initialize button on pressed function
        /// </summary>
        /// <param name="action"></param>
        public void SetButtonFunction(Action action)
            => buttonFunction = action;

        /// <summary>
        /// Set button to disable state
        /// </summary>
        /// <param name="isDisable"></param>
        public void SetToDisable(bool isDisable)
        {
            if (isOnPressed && !isDisable) return;

            if (isDisable)
                isOnPressed = false;

            this.isDisable = isDisable;
            _buttonImage.color = this.isDisable ? _colorBlock.disabledColor : _colorBlock.normalColor;
        }

        /// <summary>
        /// Function handler 
        /// when player pressed the button (_buttonImage)
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (isDisable)
                return;

            buttonFunction?.Invoke();

            pressDelayTimer = 0f;
            isOnPressed = true;

            _buttonImage.color = _colorBlock.pressedColor;
        }

        /// <summary>
        /// Function handler 
        /// when player release they pointer
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            isOnPressed = false;
            _buttonImage.color = isDisable ? _colorBlock.disabledColor : _colorBlock.normalColor;
        }

        /// <summary>
        /// /// <summary>
        /// on listen input player interupt state changed
        /// </summary>
        /// <param name="state"> true: interupt to listen player input / false: back to listen player input </param>
        private void OnInteruptInputListenerStateChanged(bool state)
        {
            isOnPressed = false;
            isDisable = state;
        }
    }
}
