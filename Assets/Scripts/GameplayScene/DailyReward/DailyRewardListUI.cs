namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    using UnityEngine.Events;

    public class DailyRewardListUI : MonoBehaviour
	{
		/// <summary>
		/// Day string
		/// </summary>
		private const string DAY_STR = "Day ";

		/// <summary>
		/// Image for reward icon
		/// </summary>
		[SerializeField] private Image _iconImage;

		/// <summary>
		/// Button to claim this reward
		/// </summary>
		[SerializeField] private Button _claimButton;

		/// <summary>
		/// day index text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _dayIdxText;

		/// <summary>
		/// reward amount text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _rewardAmountText;

		/// <summary>
		/// claimable this daily reward indicator
		/// </summary>
		[SerializeField] private GameObject _claimableIndicator;

		/// <summary>
		/// Image base frame
		/// </summary>
		[SerializeField] private Image _basePanel;

		/// <summary>
		/// unclailmed base frame sprite
		/// </summary>
		[SerializeField] private Sprite _unclaimedBase;

		/// <summary>
		/// claimed base frame sprite
		/// </summary>
		[SerializeField] private Sprite _claimedBase;

		/// <summary>
		/// unclaimed text color
		/// </summary>
		[SerializeField] private Color _unclaimedTextColor;

		/// <summary>
		/// claimed text color
		/// </summary>
		[SerializeField] private Color _claimedTextColor;

		/// <summary>
		/// claimed check image indicator
		/// </summary>
		[SerializeField] private GameObject _checkImage;

		/// <summary>
		/// Prize data that this daily reward list ui handled
		/// </summary>
		private DailyRewardData prizeData;

		/// <summary>
		/// Initialize reward data
		/// </summary>
		/// <param name="prizeData"> reward data </param>
		/// <param name="onClaimed"> on click button action </param>
		public void Initialize(DailyRewardData prizeData, UnityAction<DailyRewardListUI, DailyRewardData> onClaimed)
        {
			this.prizeData = prizeData;
			_dayIdxText.SetText(DAY_STR + this.prizeData.DayIdx);
			_rewardAmountText.SetText(Utility.StaticCurrencyStringConverison.GetString(this.prizeData.RewardItem.GetAmount()));
			_iconImage.sprite = Utility.StaticImageDictionary.Instance.GetImageSpriteByID(this.prizeData.RewardItem.GetRewardID());


			_claimButton.onClick.AddListener(() => onClaimed?.Invoke(this, this.prizeData));
        }

		/// <summary>
		/// Set ui elements
		/// </summary>
		/// <param name="lastClaimedDayIdx"> user last claimed reward idx </param>
		/// <param name="isCurrentDailyRewardClaimed"> is user already claimed current daily reward </param>
		public void SetUIElement(int lastClaimedDayIdx, bool isCurrentDailyRewardClaimed)
		{
			bool isThisRewardAlreadyClaimed = prizeData.DayIdx - 1 < lastClaimedDayIdx;

			_checkImage.SetActive(isThisRewardAlreadyClaimed);

			_basePanel.sprite = isThisRewardAlreadyClaimed ? _claimedBase : _unclaimedBase;
			_dayIdxText.color = isThisRewardAlreadyClaimed ? _claimedTextColor : _unclaimedTextColor;
			_rewardAmountText.color = isThisRewardAlreadyClaimed ? _claimedTextColor : _unclaimedTextColor;

			bool isClaimable = !isCurrentDailyRewardClaimed && prizeData.DayIdx - 1 == lastClaimedDayIdx;
			_claimButton.interactable = isClaimable;
			_claimableIndicator.SetActive(isClaimable);
		}
	}
}
