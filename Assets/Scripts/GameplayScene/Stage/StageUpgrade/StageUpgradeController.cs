namespace Project.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
	using UnityEngine.UI;


	public class StageUpgradeController : MonoBehaviour
	{
		/// <summary>
		/// stage load data container
		/// </summary>
		[SerializeField] private SaveData.SO_StageLoadData _stageData;
		
		/// <summary>
		/// upgrade effect data container
		/// </summary>
		[SerializeField] private SO_StageUpgradeEffectsCollections _upgradeEffectCollections;

		/// <summary>
		/// Upgrade stage UI
		/// </summary>
		[SerializeField] private GameObject _upgradeUI;
		
		/// <summary>
		/// indicator to indicate is player available to upgrade stage
		/// </summary>
		[SerializeField] private GameObject _availableToUpgradeIndicator;

		/// <summary>
		/// Button to open upgrade stage ui
		/// </summary>
		[SerializeField] private Button _openUI;

		/// <summary>
		/// Button to close upgrade stage ui
		/// </summary>
		[SerializeField] private Button _closeUI;

		/// <summary>
		/// stage upgrade ui list template
		/// </summary>
		[SerializeField] private StageUpgradeUIList _listTemplate;

		/// <summary>
		/// stage upgrade ui list parent
		/// </summary>
		[SerializeField] private Transform _listParent;

		/// <summary>
		/// list of upgrade ui list
		/// </summary>
		private List<StageUpgradeUIList> upgradeList = new List<StageUpgradeUIList>();

		/// <summary>
		/// is ui active state
		/// </summary>
		private bool isActiveUI;

        private void Awake()
        {
			PlayerWallet.OnCurrencyUpdate += SetUIElements;

			_openUI.onClick.AddListener(() => SetActiveUI(true));
			_closeUI.onClick.AddListener(() => SetActiveUI(false));
		}

        private IEnumerator Start()
        {
			LoadingUIController.Instance.Loading();
			
			yield return new WaitForSeconds(0.5f); // let this initialize data as the last data to initialize
			InitializeData(); // this initialize may activate some effect if player have to unlock some stage upgrade effect

			LoadingUIController.Instance.FinishLoading();
		}

        private void OnDestroy()
        {
			PlayerWallet.OnCurrencyUpdate -= SetUIElements;
		}

        /// <summary>
        /// Initialize upgrade data
        /// </summary>
        private void InitializeData()
        {
			var stageUpgradeData = _stageData.CurrentStageUpgradeData;
			var stageSaveData = SaveData.UserStageDataManager.Instance.GetStageDataByID(stageUpgradeData.StageID);
			
			var upgradeSaveData = stageSaveData == null ? new List<int>() :
				stageSaveData.UpgradeLevel == null ? new List<int>() : stageSaveData.UpgradeLevel;


			for (int i = 0; i < stageUpgradeData.UpgradeDataCount; i++)
			{
				var upgradeData = stageUpgradeData.GetUpgradeDataByIdx(i);
				var upgradeEffect = _upgradeEffectCollections.GetUpgradeEffectByID(upgradeData.EffectCode);
				
				if (upgradeSaveData.Contains(upgradeData.Level))
					upgradeEffect.ActivateEffect();
				else
				{
					StageManager.Instance.SetStageData(upgradeEffect);

					StageUpgradeUIList list = Instantiate(_listTemplate, _listParent);
					list.Initialize(upgradeData, upgradeEffect.GetIcon());
					upgradeList.Add(list);
				}
			}

			SetUIElements(Currency.ID.Coins, StageManager.Instance.GetPlayerCoinAmount());
        }

		/// <summary>
		/// on player coins update
		/// </summary>
		/// <param name="currencyID"> currency id </param>
		/// <param name="coinsAmount"> currency amount </param>
		private void SetUIElements(Currency.ID currencyID, long coinsAmount)
        {
			if (isActiveUI) return;

			if (currencyID != Currency.ID.Coins)
				return;

			foreach (var upgrade in upgradeList)
			{
				if (upgrade.IsPurchasable)
				{
					_availableToUpgradeIndicator.SetActive(true);
					return;
				}
			}

			_availableToUpgradeIndicator.SetActive(false);
		}

		/// <summary>
		/// Set active ui
		/// </summary>
		/// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveUI(bool state)
        {
			_availableToUpgradeIndicator.SetActive(false);

			foreach (var upgradeUI in upgradeList)
			{
				upgradeUI.OnStageUpgradeUIActiveStateChange(state);

				if (!state && upgradeUI.IsPurchasable)
					_availableToUpgradeIndicator.SetActive(true);
			}

			isActiveUI = state;
			_upgradeUI.SetActive(state);
        }
    }
}
