namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


	public class StageManager : MonoBehaviour
	{
        /// <summary>
        /// Data load container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _loadData;

        /// <summary>
        /// List of kitchen data in current stage played
        /// </summary>
        private List<SO_BatchKitchenLevelData> _kitchensDatas = new List<SO_BatchKitchenLevelData>();

        /// <summary>
        /// List of upgrade effect in current stage played
        /// </summary>
        private List<SO_StageUpradeEffect> _upgradeEffects = new List<SO_StageUpradeEffect>();

        /// <summary>
        /// Singleton
        /// </summary>
        private static StageManager instance;

        /// <summary>
        /// Singleton
        /// </summary>
        public static StageManager Instance => instance;

        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Set kitchen in current stage played
        /// </summary>
        /// <param name="kitchenData"> kitchen data </param>
        public void SetStageData(SO_BatchKitchenLevelData kitchenData) => _kitchensDatas.Add(kitchenData);

        /// <summary>
        /// Set upgrade effect in current stage played
        /// </summary>
        /// <param name="upgradeEffect"> upgrade effect data </param>
        public void SetStageData(SO_StageUpradeEffect upgradeEffect) => _upgradeEffects.Add(upgradeEffect);

        /// <summary>
        /// Get specific kitchen data that handle dish id
        /// </summary>
        /// <param name="dishID"> target dish id </param>
        /// <returns> kitchen data </returns>
        public SO_BatchKitchenLevelData GetKitchenDataByDishID(string dishID) 
            => _kitchensDatas.Find(x => string.Equals(x.DishData.DishID, dishID));

        /// <summary>
        /// Get current player coin amount
        /// </summary>
        /// <returns> player coin amount </returns>
        public long GetPlayerCoinAmount() 
            => PlayerWallet.GetCurrentCurrency(Currency.ID.Coins, _loadData.CurrentStageID);

        /// <summary>
        /// Player income
        /// </summary>
        /// <param name="coinAmount"> coin amount that player receive </param>
        /// <param name="isSaveImmediately"> save immedeately state, it should be true if from reward / purchsing </param>
        public void PlayerCoinIncome(long coinAmount, bool isSaveImmediately = false)
        {
            PlayerWallet.PlayerIncome(
                new CurrencyTransmission()
                {
                    CurrencyID = Currency.ID.Coins,
                    StageID = _loadData.CurrentStageID,
                    Amount = coinAmount
                },
                isSaveImmediately
                );
        }

        /// <summary>
        /// Upgrade kitchen station
        /// </summary>
        /// <param name="dishID"> target kitchen station that handle dish id </param>
        /// <param name="nextLevel"> next level data </param>
        /// <returns> result true / false : success / failed upgrade stage </returns>
        public bool IsUpgradeSuccess(string dishID, KitchenLevelData nextLevel)
        {
            if (PlayerWallet.PlayerPayment(new CurrencyTransmission()
            {
                CurrencyID = Currency.ID.Coins,
                StageID = _loadData.CurrentStageID,
                Amount = nextLevel.UpgradeCost
            }))
            {
                SaveData.UserStageDataManager.Instance.UpdateData(_loadData.CurrentStageID, dishID, nextLevel.Level);

                var kitchenData = _kitchensDatas.Find(x => string.Equals(x.DishData.DishID, dishID));
                StageEventsManager.SetKitchenLevel(
                    dishID,
                    kitchenData.GetLevelData(nextLevel.Level),
                    kitchenData.GetLevelData(nextLevel.Level + 1)
                    );

                return true;
            }
            else
            {
                FloatingTextPool.Instance.ShowFloatingText(
                        Utility.StaticConstantDictionary.NOT_ENOUGH_RESOURCES_MESSAGE,
                        Input.mousePosition,
                        FloatingTextObj.Position_State.Screen,
                        FloatingTextObj.Text_State.Invalid
                    );

                return false;
            }
        }

        /// <summary>
        /// Upgrade stage
        /// </summary>
        /// <param name="upgradeData"> upgrade data </param>
        /// <returns> result true / false : success / failed upgrade stage </returns>
        public bool IsUpgradeSuccess(StageUpgradeData upgradeData)
        {
            if (PlayerWallet.PlayerPayment(new CurrencyTransmission()
            {
                CurrencyID = Currency.ID.Coins,
                StageID = _loadData.CurrentStageID,
                Amount = upgradeData.Price
            }))
            {
                SaveData.UserStageDataManager.Instance.UpdateData(_loadData.CurrentStageID, upgradeData.Level);
                _upgradeEffects.Find(x => string.Equals(x.UpgradeEffectID, upgradeData.EffectCode)).ActivateEffect();
                return true;
            }
            else
            {
                FloatingTextPool.Instance.ShowFloatingText(
                    Utility.StaticConstantDictionary.NOT_ENOUGH_RESOURCES_MESSAGE,
                    Input.mousePosition,
                    FloatingTextObj.Position_State.Screen,
                    FloatingTextObj.Text_State.Invalid
                    );

                return false;
            }
        }

        /// <summary>
        /// Moving stage
        /// </summary>
        /// <param name="currentStageData"> current stage </param>
        /// <param name="nextStageData"> next stage </param>
        /// <returns> result true / false : success / failed moving stage </returns>
        public bool IsMovingStageSuccess(StageDefaultData currentStageData, StageDefaultData nextStageData)
        {
            if (PlayerWallet.PlayerPayment(
                new CurrencyTransmission()
                {
                    CurrencyID = Currency.ID.Coins,
                    StageID = currentStageData.StageID,
                    Amount = currentStageData.FinishStagePrice
                }))
            {
                SaveData.UserStageDataManager.Instance.AddStageData(nextStageData.StageID, nextStageData.StartingCoin);
                PlayerWallet.AddNewPlayerStageCoin(nextStageData.StageID, nextStageData.StartingCoin);

                _loadData.SetStageID(nextStageData.StageID);
                UnityEngine.SceneManagement.SceneManager.LoadScene(Utility.StaticConstantDictionary.SCENE_GAMEPLAY_IDX);

                return true;
            }
            else
            {
                FloatingTextPool.Instance.ShowFloatingText(
                        Utility.StaticConstantDictionary.NOT_ENOUGH_RESOURCES_MESSAGE,
                        Input.mousePosition,
                        FloatingTextObj.Position_State.Screen,
                        FloatingTextObj.Text_State.Invalid
                    );

                return false;
            }
        }

        /// <summary>
        /// Save this stage progression data
        /// </summary>
        public void SaveStageDataProgression()
        {
            SaveData.UserStageDataManager.Instance.SaveDataImmdiately();
        }
    }
}
