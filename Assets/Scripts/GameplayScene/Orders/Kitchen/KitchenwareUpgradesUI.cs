namespace Project.Gameplay
{
	using System;
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;


	public class KitchenwareUpgradesUI : MonoBehaviour
	{
		/// <summary>
		/// Max level lable
		/// </summary>
		private const string MAX_LEVEL_STR = "Max Level";

		/// <summary>
		/// String level initial
		/// </summary>
		private const string LEVEL_STR = "Lv.";

		/// <summary>
		/// Second initial
		/// </summary>
		private const string SECOND_STR = "s";

		/// <summary>
		/// kitchen upgrade UI
		/// </summary>
		[SerializeField] private GameObject _ui;

		/// <summary>
		/// indicator that will show up when kitchen can be to upgrade
		/// </summary>
		[SerializeField] private GameObject _upgradeIndicator;

		/// <summary>
		/// current level text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _txtCurrentLevel;

		/// <summary>
		/// dish name text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _txtDishName;
		
		/// <summary>
		/// list of stars for each phase in this kitchen
		/// </summary>
		[SerializeField] private Image[] _starsImage;

		/// <summary>
		/// blank star sprite
		/// </summary>
		[SerializeField] private Sprite _blankStar;

		/// <summary>
		/// filled star sprite
		/// </summary>
		[SerializeField] private Sprite _fillStar;
		
		/// <summary>
		/// level progress bar to indicate level in each phase
		/// </summary>
		[SerializeField] private Slider _levelProgresBar;

		/// <summary>
		/// reward icon that will player get in the end of phase
		/// </summary>
		[SerializeField] private Image _imgRewardPhaseIcon;

		/// <summary>
		/// reward amount text that will player get in the end of phase
		/// </summary>
		[SerializeField] private TextMeshProUGUI _txtRewardPhaseAmount;

		/// <summary>
		/// current kitchen level profit
		/// </summary>
		[SerializeField] private TextMeshProUGUI _txtCurrentProfit;

		/// <summary>
		/// current kitchen process time
		/// </summary>
		[SerializeField] private TextMeshProUGUI _txtCurrentProcessTime;

		/// <summary>
		/// upgrade button
		/// </summary>
		[SerializeField] private HoldTappingButtonInput _upgradeButton;

		/// <summary>
		/// upgrade cost label
		/// </summary>
		[SerializeField] private TextMeshProUGUI _txtUpgradeCost;

		/// <summary>
		/// coin icon in upgrade cost label
		/// </summary>
		[SerializeField] private GameObject _coinIcon;

		/// <summary>
		/// get current dish profit
		/// dish profit will be accumulate from kitchen
		/// </summary>
		private Func<long> GetDishProfit;

		/// <summary>
		/// get dish process time
		/// dish process time will be accumulate from kitchen
		/// </summary>
		private Func<float> GetDishProcessTime;

		/// <summary>
		/// dish name
		/// </summary>
		private string _dishName;

		/// <summary>
		/// total of level upgrade phase in this kitchen
		/// </summary>
		private int _totalPhaseCount;

		/// <summary>
		/// kitchen current level
		/// </summary>
		private int currentLevel;

		/// <summary>
		/// kitchen current level upgrade phase
		/// </summary
		private int currentPhase;

		/// <summary>
		/// current kitchen phase start level
		/// </summary>
		private int currentPhaseStartLevel;

		/// <summary>
		/// current kitchen phase last level
		/// </summary>
		private int currentPhaseLastLevel;

		/// <summary>
		/// current upgrade cost to level up to the next level
		/// </summary>
		private long currentUpgradeCost;

		/// <summary>
		/// is max level state
		/// </summary>
		private bool isMaxLevel;

		/// <summary>
		/// is ui active state
		/// </summary>
		private bool isUIActive;


        private void Awake()
        {
			PlayerWallet.OnCurrencyUpdate += SetUpgradeIndicatorElement;
		}

        private void OnDestroy()
        {
			PlayerWallet.OnCurrencyUpdate -= SetUpgradeIndicatorElement;
		}

		/// <summary>
		/// Initialize kitchen upgrade ui
		/// </summary>
		/// <param name="dishName"> dish name that this kitchen handle </param>
		/// <param name="phaseCount"> level upgrade phase count in this kitchen upgrad data </param>
		/// <param name="getDishProfit"> accumulate dish profit function </param>
		/// <param name="getDishProcessTime"> accumulate dish process time function </param>
		/// <param name="levelUpButtonAction"> upgrade button on click events </param>
		public void Initialize(
			string dishName, 
			int phaseCount, 
			Func<long> getDishProfit, 
			Func<float> getDishProcessTime, 
			Action levelUpButtonAction)
        {
			_dishName = dishName;
			_totalPhaseCount = phaseCount;

			GetDishProfit = getDishProfit;
			GetDishProcessTime = getDishProcessTime;
			

			for (int i = 0; i < _starsImage.Length; i++)
				_starsImage[i].gameObject.SetActive(i < _totalPhaseCount);

			levelUpButtonAction += SetUIElement;
			_upgradeButton.SetButtonFunction(levelUpButtonAction);
		}

		/// <summary>
		/// Set current kitchen level
		/// </summary>
		/// <param name="currentLevel"> current kitchen level </param>
		/// <param name="currentPhase"> current phase </param>
		/// <param name="currentPhaseStartLevel"> current phase start level </param>
		/// <param name="currentPhaseLastLevel"> current phase last level </param>
		/// <param name="currentUpgradeCost"> current upgrade level cost </param>
		/// <param name="rewardPhaseAmount"> current reward phase amount </param>
		/// <param name="rewardPhaseIcon"> current reward phase amount </param>
		/// <param name="isMaxLevel"> current kitchen level is max level state </param>
		public void SetLevel(
			int currentLevel,
			int currentPhase,
			int currentPhaseStartLevel,
			int currentPhaseLastLevel,
			long currentUpgradeCost,
			string rewardPhaseAmount,
			Sprite rewardPhaseIcon,
			bool isMaxLevel
			)
        {
			this.currentLevel = currentLevel;

			this.currentPhase = currentPhase;
			this.currentPhaseStartLevel = currentPhaseStartLevel;
			this.currentPhaseLastLevel = currentPhaseLastLevel;
			this.currentUpgradeCost = currentUpgradeCost;
			this.isMaxLevel = isMaxLevel;

			_txtRewardPhaseAmount.SetText(rewardPhaseAmount);
			_imgRewardPhaseIcon.sprite = rewardPhaseIcon;

			_txtUpgradeCost.SetText(this.isMaxLevel ? 
				MAX_LEVEL_STR : Utility.StaticCurrencyStringConverison.GetString(this.currentUpgradeCost));

			_coinIcon.SetActive(!this.isMaxLevel);

			SetUpgradeIndicatorElement(Currency.ID.Coins, StageManager.Instance.GetPlayerCoinAmount());
		}

		/// <summary>
		/// Set ui elements
		/// </summary>
		private void SetUIElement()
		{
			_txtCurrentLevel.SetText(LEVEL_STR + currentLevel);
			_txtDishName.SetText(_dishName);

			_txtCurrentProfit.SetText(Utility.StaticCurrencyStringConverison.GetString(GetDishProfit()));
			_txtCurrentProcessTime.SetText(GetDishProcessTime() + SECOND_STR);

			_levelProgresBar.maxValue = currentPhaseLastLevel - currentPhaseStartLevel;
			_levelProgresBar.SetValueWithoutNotify(currentLevel - currentPhaseStartLevel);

			bool showRewardPhase = currentPhaseLastLevel > currentLevel;
			_imgRewardPhaseIcon.gameObject.SetActive(showRewardPhase);
			_txtRewardPhaseAmount.gameObject.SetActive(showRewardPhase);

			for (int i = 0; i < _totalPhaseCount; i++)
				_starsImage[i].sprite = i < currentPhase ? _fillStar : _blankStar;

			SetUpgradeIndicatorElement(Currency.ID.Coins, StageManager.Instance.GetPlayerCoinAmount());
		}

		/// <summary>
		/// Set upgrade indicator elements
		/// </summary>
		/// <param name="currencyID"> currency id </param>
		/// <param name="cointAmount"> player current currency amount </param>
		private void SetUpgradeIndicatorElement(Currency.ID currencyID, long cointAmount)
		{
			if (currencyID != Currency.ID.Coins)
				return;

			bool isCanUpgrade = !isMaxLevel && cointAmount >= currentUpgradeCost;

			_upgradeIndicator.SetActive(!isUIActive && isCanUpgrade);
			_upgradeButton.SetToDisable(!isCanUpgrade);
		}

		/// <summary>
		/// Set active ui
		/// </summary>
		/// <param name="state"> true: set active / false: deactive </param>
		public void SetActiveUI(bool state)
		{
			isUIActive = state;

			SetUIElement();
			_ui.SetActive(state);
		}
	}
}