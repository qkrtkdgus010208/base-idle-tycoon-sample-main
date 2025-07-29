namespace Project.Gameplay
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;


    public class ProgressBar : MonoBehaviour
    {
        /// <summary>
        /// progress bar max value
        /// </summary>
        private const float PROGRESS_BAR_MAX_VALUE = 1f;

        /// <summary>
        /// progress bar fill
        /// </summary>
        [SerializeField] private Image _imageProgress;

        /// <summary>
        /// callback after process finish
        /// </summary>
        private Action OnProcessFinish;

        /// <summary>
        /// Start the process
        /// </summary>
        /// <param name="progressTime"> long dish process time </param>
        /// <param name="onProcessFinish"> callback after dish on process finish </param>
        public void StartProgress(float progressTime, Action onProcessFinish)
        {
            _imageProgress.fillAmount = 0;
            gameObject.SetActive(true);

            OnProcessFinish = onProcessFinish;

            float barFillRate = PROGRESS_BAR_MAX_VALUE / progressTime;
            StartCoroutine(ProgressTimer(barFillRate, progressTime));
        }

        /// <summary>
        /// Progression
        /// </summary>
        /// <param name="barFillRate"> progression bar fill rate per frame </param>
        /// <param name="progressTime"> long dish process time </param>
        /// <returns></returns>
        private IEnumerator ProgressTimer(float barFillRate, float progressTime)
        {
            float timer = 0;

            while (timer < progressTime)
            {
                timer += Time.deltaTime;
                _imageProgress.fillAmount = timer * barFillRate;

                yield return null;
            }

            OnProcessFinish?.Invoke();
            gameObject.SetActive(false);
        }
    }
}