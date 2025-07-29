namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    

    public class MailRewardItemsListUI : MonoBehaviour, Utility.IPoolingObjects
	{
        /// <summary>
        /// Image for reward icon
        /// </summary>
        [SerializeField] private Image _rewardIcon;

        /// <summary>
        /// reward amount text output
        /// </summary>
		[SerializeField] private TextMeshProUGUI _rewardAmount;

        /// <summary>
        /// Object pool activate state
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Object pool activate state
        /// </summary>
        /// <returns> true: object active / false: object inactive </returns>
        public bool IsActive() => isActive;

        /// <summary>
        /// Set active object pool
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActive(bool state)
        {
            isActive = state;
            gameObject.SetActive(state);
        }

        /// <summary>
        /// Set mail reward element
        /// </summary>
        /// <param name="icon"> reward icon </param>
        /// <param name="amount"> reward amount </param>
        public void SetMailRewardElement(Sprite icon, string amount)
		{
            _rewardIcon.sprite = icon;
            _rewardAmount.SetText(amount);
        }

	}
}
