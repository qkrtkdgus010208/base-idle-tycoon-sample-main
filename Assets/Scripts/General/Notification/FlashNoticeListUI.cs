namespace Project
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    using System.Collections;

    public class FlashNoticeListUI : MonoBehaviour, Utility.IPoolingObjects
	{
        /// <summary>
        /// default icon
        /// </summary>
        [SerializeField] private Sprite _defaultIcon;

        /// <summary>
        /// image for icon
        /// </summary>
		[SerializeField] private Image _flashNotifIcon;

        /// <summary>
        /// text output for message
        /// </summary>
		[SerializeField] private Text _flashNotifMessageText;

        /// <summary>
        /// active time
        /// </summary>
        private YieldInstruction _flashTime = new WaitForSeconds(3f);

        /// <summary>
        /// is active state
        /// </summary>
        private bool isActive;

        /// <summary>
        /// is active state
        /// </summary>
        public bool IsActive() => isActive;

        /// <summary>
        /// Showing flash notice
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        public void SetNotice(string message, Sprite icon)
        {
            _flashNotifMessageText.text = message; // set message
            _flashNotifIcon.sprite = icon ?? _defaultIcon; // set icon, use default icon if icon is null

            SetActive(true);
            StartCoroutine(FlashNotice());
        }

        /// <summary>
        /// flash notice active timer
        /// </summary>
        /// <returns></returns>
		private IEnumerator FlashNotice()
        {
            yield return _flashTime;
            SetActive(false);
        }

        /// <summary>
        /// Set active state (Show / Hide)
        /// </summary>
        /// <param name="state"></param>
        public void SetActive(bool state)
        {
            isActive = state;
            gameObject.SetActive(state);
        }
    }
}
