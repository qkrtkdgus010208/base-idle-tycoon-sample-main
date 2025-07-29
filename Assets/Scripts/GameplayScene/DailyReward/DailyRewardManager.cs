namespace Project.Gameplay
{
	using SaveData;
	using System;
    using UnityEngine;
    using UnityEngine.UI;


	public class DailyRewardManager : MonoBehaviour
	{
		/// <summary>
		/// Available daily reward count
		/// </summary>
		private const int DAILY_REWARD_LAST_IDX = 7;

		/// <summary>
		/// daily reward data container
		/// </summary>
		[SerializeField] private SO_BatchDailyRewardData _dailyRewardData;

		/// <summary>
		/// daily reward UI
		/// </summary>
		[SerializeField] private GameObject _ui;

		/// <summary>
		/// button to open daily reward UI
		/// </summary>
		[SerializeField] private Button _openUIButton;

		/// <summary>
		/// close to open daily reward UI
		/// </summary>
		[SerializeField] private Button _closeUIButton;

		/// <summary>
		/// Available reward to claim indicator
		/// </summary>
		[SerializeField] private GameObject _availableRewardIndicator;

		/// <summary>
		/// reward list ui parent
		/// </summary>
		[SerializeField] private Transform _rewardListParent;
		
		/// <summary>
		/// reward list ui template
		/// </summary>
		[SerializeField] private DailyRewardListUI _rewardListUITemplate;

		/// <summary>
		/// reward list ui template for last idx in daily reward
		/// </summary>
		[SerializeField] private DailyRewardListUI _rewardListUILastIdxTemplate;


        private void Awake()
        {
			_openUIButton.onClick.AddListener(() => SetActiveUI(true));
			_closeUIButton.onClick.AddListener(() => SetActiveUI(false));
		}

        private void Start()
        {
			Initialize();
        }

		/// <summary>
		/// Initialize daily reward data
		/// </summary>
        private void Initialize()
        {
			var dailyRewardIdx = UserDailyLoginDataManager.Instance.CurrentDailyRewardIdx % DAILY_REWARD_LAST_IDX;
			
			var lastClaimedDailyRewardStr = UserDailyLoginDataManager.Instance.LastClaimedDailyRewardDate;
			var lastClaimedDailyRewardDate = string.IsNullOrEmpty(lastClaimedDailyRewardStr) ?
				new DateTime() : DateTime.Parse(lastClaimedDailyRewardStr);

			var currentDate = BackndServer.BackndServerTime.GetServerTime();

			bool isNewDayAfterLastClaimedDailyReward =
				currentDate.Year > lastClaimedDailyRewardDate.Year ||
				currentDate.Month > lastClaimedDailyRewardDate.Month ||
				currentDate.Day > lastClaimedDailyRewardDate.Day;

			foreach(var prize in _dailyRewardData.RewardDatas)
            {
				DailyRewardListUI list = Instantiate(
					prize.DayIdx == DAILY_REWARD_LAST_IDX ? 
					_rewardListUILastIdxTemplate : _rewardListUITemplate,
					_rewardListParent
					);

				list.Initialize(prize, OnDailyRewardClaimed);
				list.SetUIElement(dailyRewardIdx, !isNewDayAfterLastClaimedDailyReward);
            }

			_availableRewardIndicator.SetActive(isNewDayAfterLastClaimedDailyReward);
		}

		/// <summary>
		/// Events when daily reward claimed
		/// </summary>
		/// <param name="ui"> daily reward list ui that handle current claimed reward </param>
		/// <param name="prizeData"> daily reward </param>
		private void OnDailyRewardClaimed(DailyRewardListUI ui, DailyRewardData prizeData)
        {
			prizeData.RewardItem.ReceiveItem();
			UserDailyLoginDataManager.Instance.UpdateLoginDayIdx(prizeData.DayIdx);

			ui.SetUIElement(prizeData.DayIdx, true);
			_availableRewardIndicator.SetActive(false);
		}

		/// <summary>
		/// Set active daily reward UI
		/// </summary>
		/// <param name="state"> true: set active / false: deactive </param>
		private void SetActiveUI(bool state)
		{
			_ui.SetActive(state);
		}
	}
}
