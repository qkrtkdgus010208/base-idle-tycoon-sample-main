namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    using System;
    using UnityEngine.EventSystems;

    public class RewardConfirmationController : MonoBehaviour
	{
        /// <summary>
        /// Reward confirmation ui
        /// </summary>
		[SerializeField] private GameObject _ui;

        /// <summary>
        /// Image for reward icon
        /// </summary>
		[SerializeField] private Image _rewardIcon;

        /// <summary>
        /// reward amount text output
        /// </summary>
		[SerializeField] private TextMeshProUGUI _rewardAmountText;

        /// <summary>
        /// Confirmation button
        /// </summary>
		[SerializeField] private Button _confirmButton;

        /// <summary>
        /// Events to show reward confirmation
        /// </summary>
        public static Action<Sprite, string, Action> ShowRewardConfirmation;



        private void Awake()
        {
            ShowRewardConfirmation = ShowRewardConfirmationUI;
        }

        /// <summary>
        /// Show reward confirmation
        /// </summary>
        /// <param name="rewardIcon"> reward icon </param>
        /// <param name="rewardAmount"> reward amount </param>
        /// <param name="onConfirm"> confirm button on click action </param>
        private void ShowRewardConfirmationUI(Sprite rewardIcon, string rewardAmount, Action onConfirm)
        {
            _rewardIcon.sprite = rewardIcon;
            _rewardAmountText.SetText(rewardAmount);

            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() => { onConfirm?.Invoke(); SetActiveUI(false); });

            InputListener.SetInteruptInputListener(true);
            SetActiveUI(true);
        }

        private void SetActiveUI(bool state)
        {
            InputListener.SetInteruptInputListener(state);
            _ui.SetActive(state);
        }
    }
}
