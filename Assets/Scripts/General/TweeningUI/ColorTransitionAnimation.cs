namespace Project
{
    using UnityEngine;
    using UnityEngine.UI;
	using DG.Tweening;
    using System;

    public class ColorTransitionAnimation : AbstractTweeningObjectUI
    {
        /// <summary>
        /// transition graphic target
        /// </summary>
        [SerializeField] private Graphic _tweeningUI;

        /// <summary>
        /// transition duration
        /// </summary>
        [SerializeField] private float _duration;

        /// <summary>
        /// transition start color
        /// </summary>
        [SerializeField] private Color _startColor;

        /// <summary>
        /// transition end color
        /// </summary>
        [SerializeField] private Color _endColor;


        /// <summary>
        /// is animation play on as soon as the object active state
        /// </summary>
        [SerializeField] private bool _isPlayOnStart;

        /// <summary>
        /// is animation loop state
        /// </summary>
        [SerializeField] private bool _isLoop;

        /// <summary>
        /// is do reverse animation state
        /// </summary>
        [SerializeField] private bool _isReverse;


        private void Start()
        {
            if (_isPlayOnStart) // if play on start, do the animation on start
                DoAnimationUI();
        }

        /// <summary>
        /// Do color transition (startColor to endColor)
        /// </summary>
        public override void DoAnimationUI()
        {
            // dotween color transition from startColor to endColor, x as color result
            DOTween.To(() => _startColor, x => _tweeningUI.color = x, _endColor, _duration)
                .onComplete += () => { if (_isLoop || _isReverse) DoAnimationReverse(); };
        }

        /// <summary> 
        /// Do color transition reverse (endColor to startColor)
        /// </summary>
        public void DoAnimationReverse()
        {
            // dotween color transition from endColor to startColor, x as color result
            DOTween.To(() => _endColor, x => _tweeningUI.color = x, _startColor, _duration)
                .onComplete += () => { if (_isLoop) DoAnimationUI(); };
        }
    }
}
