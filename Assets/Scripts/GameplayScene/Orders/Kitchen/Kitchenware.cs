namespace Project.Gameplay
{
    using System;
    using UnityEngine;


    public class Kitchenware : MonoBehaviour
    {
        /// <summary>
        /// Kitchen upgrade reward code separator
        /// </summary>
        private const char REWARD_ID_SEPARATOR = '_';

        /// <summary>
        /// Activate requirement message
        /// </summary>
        private const string ACTIVATE_REQUIREMENTS_MESSAGE = "Open {0} station first";

        /// <summary>
        /// Kitchen station table sprite
        /// Deactive when kitchen is not active yet (Lv 0)
        /// </summary>
        [SerializeField] private GameObject _tableSpriteObject;

        /// <summary>
        /// Place holder sprite
        /// Active when kitchen is not active yet (Lv 0)
        /// </summary>
        [SerializeField] private GameObject _placeHolderSpriteObject;

        /// <summary>
        /// Input listener area to show upgrade UI
        /// </summary>
        [SerializeField] private InteractKitchenTable[] _interactTable;

        /// <summary>
        /// Kitchen table that will be operate by staff
        /// </summary>
        [SerializeField] private KitchenTable[] _kitchenTables;

        /// <summary>
        /// Kitchen upgrade UI
        /// Active if player input in InteractKitchenTable
        /// </summary>
        [SerializeField] private KitchenwareUpgradesUI _kitchenUpgradeUI;

        /// <summary>
        /// This kitchen station level data from database
        /// </summary>
        private SO_BatchKitchenLevelData _batchKitchenData;

        /// <summary>
        /// current kitchen station level
        /// </summary>
        private KitchenLevelData currentLevel;

        /// <summary>
        /// next kitchen station level
        /// </summary>
        private KitchenLevelData nextLevel;

        /// <summary>
        /// Dish data that This kitchen station handle
        /// </summary>
        [SerializeField] private SO_DishData _dishData;

        /// <summary>
        /// Dish data that This kitchen station handle
        /// </summary>
        public SO_DishData DishData => _dishData;

        /// <summary>
        /// Dish id that This kitchen station handle
        /// </summary>
        public string DishID => DishData.DishID;

        /// <summary>
        /// Kitchen station active state
        /// true if kitchen active (Lv > 0)
        /// false if kitchen active (Lv == 0)
        /// </summary>
        private bool isKitchenActive;

        /// <summary>
        /// Kitchen station active state
        /// true if kitchen active (Lv > 0)
        /// false if kitchen active (Lv == 0)
        /// </summary>
        public bool IsKitchenActive => isKitchenActive;

        /// <summary>
        /// Checking is current level is max level
        /// </summary>
        private bool IsMaxLevel
            => currentLevel.Level >= nextLevel.Level;

        /// <summary>
        /// current general profit bonus value
        /// </summary>
        private float generalProfitBonusMultiply;

        /// <summary>
        /// current kitchen profit bonus value
        /// </summary>
        private float kitchenProfitBonusMultiply;

        /// <summary>
        /// current kitchen reduce process time value
        /// </summary>
        private float kitchenReduceProcessTimeMultiply;

        /// <summary>
        /// Requirement kitchen to active before activate this kitchen
        /// </summary>
        private Kitchenware _kitchenRequirementForActivate;

        /// <summary>
        /// checking is requirement to activate this kitchen station is met
        /// </summary>
        private bool ActivateRequirementsCheck
            => _kitchenRequirementForActivate == null || _kitchenRequirementForActivate.IsKitchenActive;



        private void Awake()
        {
            StageEventsManager.GetKitchenTablePathPoint += GetKitchenTablePathPoint;
            StageEventsManager.OperateKitchenTable += OperateKitchenTable;
            StageEventsManager.OnGeneralProfitBonusChanged += SetGeneralProfitBonus;
            StageEventsManager.OnKitchenProfitBonusChanged += SetKitchenProfitBonus;
            StageEventsManager.OnKitchenReduceTimeChanged += SetKitchenReduceProcessTime;
            StageEventsManager.SetKitchenLevel += LevelUpgrade;
        }

        private void OnDestroy()
        {
            StageEventsManager.GetKitchenTablePathPoint -= GetKitchenTablePathPoint;
            StageEventsManager.OperateKitchenTable -= OperateKitchenTable;
            StageEventsManager.OnGeneralProfitBonusChanged -= SetGeneralProfitBonus;
            StageEventsManager.OnKitchenProfitBonusChanged -= SetKitchenProfitBonus;
            StageEventsManager.OnKitchenReduceTimeChanged -= SetKitchenReduceProcessTime;
            StageEventsManager.SetKitchenLevel -= LevelUpgrade;
        }

        /// <summary>
        /// Initialize kitchen station
        /// </summary>
        /// <param name="currentLevel"> kitchen current level </param>
        /// <param name="kitchenRequirementForActive"> requirement kitchen station before activate this kitchen station </param>
        public void SetupKitchen(int currentLevel, Kitchenware kitchenRequirementForActive)
        {
            _batchKitchenData = StageManager.Instance.GetKitchenDataByDishID(DishID);
            
            _kitchenRequirementForActivate = kitchenRequirementForActive;
            this.currentLevel = _batchKitchenData.GetLevelData(currentLevel);
            nextLevel = _batchKitchenData.GetLevelData(currentLevel + 1);

            int phaseCount = 0;
            int currentPhase = 0;
            int currentPhaseStartLevel = 0;
            int currentPhaseLastLevel = _batchKitchenData.MaxLevel;
            string[] rewardCode = _batchKitchenData.GetLevelData(_batchKitchenData.MaxLevel).RewardId.Split(REWARD_ID_SEPARATOR);

            for (int i = 1; i <= _batchKitchenData.MaxLevel; i++)
            {
                if (_batchKitchenData.GetLevelData(i).IsNextPhase)
                {
                    phaseCount++;

                    if (i <= currentLevel)
                    {
                        currentPhase = phaseCount;
                        currentPhaseStartLevel = i;
                    }
                    else if (i - 1 >= currentLevel && currentPhaseLastLevel == _batchKitchenData.MaxLevel)
                    {
                        var lastPhaseLevel = _batchKitchenData.GetLevelData(i - 1);
                        rewardCode = lastPhaseLevel.RewardId.Split(REWARD_ID_SEPARATOR);
                        currentPhaseLastLevel = lastPhaseLevel.Level;
                    }
                }
            }

            _kitchenUpgradeUI.Initialize(
                DishData.DishName, 
                phaseCount, 
                CalculateCurrentProfit, 
                CalculateCurrentProcessTime,
                UpgradeKitchen);

            _kitchenUpgradeUI.SetLevel(
                this.currentLevel.Level,
                currentPhase,
                currentPhaseStartLevel,
                currentPhaseLastLevel,
                nextLevel.UpgradeCost,
                rewardCode[2],
                Utility.StaticImageDictionary.Instance.GetImageSpriteByID(rewardCode[1]),
                IsMaxLevel
                );

            SetGeneralProfitBonus(StageEventsManager.GetCurrentGeneralProfitBonus());
            SetKitchenProfitBonus(DishID, StageEventsManager.GetKitchenProfitBonus(DishID));
            SetKitchenReduceProcessTime(DishID, StageEventsManager.GetKitchenReduceTime(DishID));

            SetInteractTable();
            SetKitchenTable();
        }

        /// <summary>
        /// Set interact listener for this kitchen station
        /// </summary>
        private void SetInteractTable()
        {
            foreach (var table in _interactTable)
                table.SetPlayerInteractAction(
                    () => _kitchenUpgradeUI.SetActiveUI(true),
                    () => _kitchenUpgradeUI.SetActiveUI(false)
                    );
        }

        /// <summary>
        /// Initialize all kitchen table in this kitchen station
        /// </summary>
        private void SetKitchenTable()
        {
            int tableCount = 0;

            for (int i = 1; i <= currentLevel.Level; i++)
            {
                if (_batchKitchenData.GetLevelData(i).IsOpenMoreTable)
                    tableCount++;
            }

            if (tableCount > 0)
                ActivateKitchen();

            for (int i = 0; i < _kitchenTables.Length; i++)
                _kitchenTables[i].SetActiveTable(i < tableCount);
        }

        /// <summary>
        /// Get kitchen available state
        /// true: at least one table is not isBusy
        /// false: all table isBusy
        /// </summary>
        public bool IsKitchenAvailable
        {
            get
            {
                foreach (KitchenTable table in _kitchenTables)
                {
                    if (!table.IsTableActive) continue;

                    if (!table.IsBusy)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get available table in this kitchen station, to staff to operate the kitchen table
        /// </summary>
        /// <param name="id"> target kitchen dish id </param>
        /// <param name="onGetTablePath"> callback after getting the available table </param>
        private void GetKitchenTablePathPoint(string id, Action<int, Vector2, float> onGetTablePath)
        {
            if (!string.Equals(DishID, id)) return;

            for (int i = 0; i < _kitchenTables.Length; i++)
            {
                if (!_kitchenTables[i].IsTableActive) continue;

                if (!_kitchenTables[i].IsBusy)
                {
                    _kitchenTables[i].SetBusy();
                    onGetTablePath?.Invoke(i, _kitchenTables[i].PointPosition, _kitchenTables[i].transform.position.x);
                    break;
                }
            }
        }

        /// <summary>
        /// Operate specific kitchen table that have kitchenTableIdx
        /// </summary>
        /// <param name="id"> target kitchen dish id </param>
        /// <param name="kitchenTableIdx"> kitchen table idx in this kitchen station </param>
        /// <param name="onProcessFinish"> callback after operate process finish </param>
        private void OperateKitchenTable(string id, int kitchenTableIdx, Action<long> onProcessFinish)
        {
            if (!string.Equals(DishID, id)) return;

            _kitchenTables[kitchenTableIdx].ProcessTable(
                CalculateCurrentProcessTime(), () => onProcessFinish?.Invoke(CalculateCurrentProfit()));
        }

        /// <summary>
        /// Set current general profit bonus
        /// </summary>
        /// <param name="currentGeneralProfitBonus"> general profit amount value </param>
        private void SetGeneralProfitBonus(float currentGeneralProfitBonus) 
            => generalProfitBonusMultiply = currentGeneralProfitBonus;

        /// <summary>
        /// Set kitchen profit bonus
        /// </summary>
        /// <param name="dishID"> target kitchen dish id </param>
        /// <param name="bonusProfitAmount"> bonus profit amount value </param>
        private void SetKitchenProfitBonus(string dishID, float bonusProfitAmount)
        {
            if (!string.Equals(DishID, dishID)) return;
            kitchenProfitBonusMultiply = bonusProfitAmount;
        }

        /// <summary>
        /// Set reduce process time value
        /// </summary>
        /// <param name="dishID"> target kitchen dish id </param>
        /// <param name="reduceAmount"> reduce amount value </param>
        private void SetKitchenReduceProcessTime(string dishID, float reduceAmount)
        {
            if (!string.Equals(DishID, dishID)) return;
            kitchenReduceProcessTimeMultiply = reduceAmount;
        }
        
        /// <summary>
        /// Calculate current profit
        /// </summary>
        /// <returns> current profit </returns>
        public long CalculateCurrentProfit()
            => (long)(currentLevel.DishProfit * (1f + kitchenProfitBonusMultiply + generalProfitBonusMultiply));

        /// <summary>
        /// Claculate current process time
        /// </summary>
        /// <returns> current process time </returns>
        public float CalculateCurrentProcessTime()
            => DishData.DishProcessTime * (1f - kitchenReduceProcessTimeMultiply);

        /// <summary>
        /// Upgrade Kitchen
        /// </summary>
        private void UpgradeKitchen()
        {
            if (!ActivateRequirementsCheck)
            {
                FloatingTextPool.Instance.ShowFloatingText(
                    string.Format(ACTIVATE_REQUIREMENTS_MESSAGE, _kitchenRequirementForActivate.DishData.DishName),
                    Input.mousePosition,
                    FloatingTextObj.Position_State.Screen,
                    FloatingTextObj.Text_State.Normal
                    );

                return;
            }

            if (IsMaxLevel)
                return;

            StageManager.Instance.IsUpgradeSuccess(_dishData.DishID, nextLevel);
        }

        /// <summary>
        /// Upgrade kitchen level
        /// </summary>
        /// <param name="dishID"> target kitchen dish id </param>
        /// <param name="currentLevelData"> level data after upgrade </param>
        /// <param name="nextLevelData"> next level data </param>
        private void LevelUpgrade(string dishID, KitchenLevelData currentLevelData, KitchenLevelData nextLevelData)
        {
            if (!string.Equals(DishID, dishID))
                return;

            currentLevel = currentLevelData;
            nextLevel = nextLevelData;
            
            if (currentLevel.IsOpenMoreTable)
                OpenMoreTable();

            if (!string.IsNullOrEmpty(currentLevel.RewardId))
                SetUpgradeReward(currentLevel.RewardId);

            int phaseCount = 0;
            int currentPhase = 0;
            int currentPhaseStartLevel = 0;
            int currentPhaseLastLevel = _batchKitchenData.MaxLevel;
            string[] rewardCode = _batchKitchenData.GetLevelData(_batchKitchenData.MaxLevel).RewardId.Split(REWARD_ID_SEPARATOR);
            
            for (int i = 1; i <= _batchKitchenData.MaxLevel; i++)
            {
                if (_batchKitchenData.GetLevelData(i).IsNextPhase)
                {
                    phaseCount++;

                    if (i <= currentLevel.Level)
                    {
                        currentPhase = phaseCount;
                        currentPhaseStartLevel = i;
                    }
                    else if (i - 1 >= currentLevel.Level && currentPhaseLastLevel == _batchKitchenData.MaxLevel)
                    {
                        var lastPhaseLevel = _batchKitchenData.GetLevelData(i - 1);
                        rewardCode = lastPhaseLevel.RewardId.Split(REWARD_ID_SEPARATOR);
                        currentPhaseLastLevel = lastPhaseLevel.Level;
                        break;
                    }
                }
            }

            _kitchenUpgradeUI.SetLevel(
                currentLevel.Level, 
                currentPhase, 
                currentPhaseStartLevel, 
                currentPhaseLastLevel,
                nextLevel.UpgradeCost,
                rewardCode[2],
                Utility.StaticImageDictionary.Instance.GetImageSpriteByID(rewardCode[1]),
                IsMaxLevel
                );
        }

        /// <summary>
        /// Open more kitchen station table
        /// </summary>
        private void OpenMoreTable()
        {
            if (!IsKitchenActive)
                ActivateKitchen();

            foreach (KitchenTable table in _kitchenTables)
            {
                if (table.IsTableActive) continue;

                table.SetActiveTable(true);
                break;
            }
        }

        /// <summary>
        /// Set active kitchen
        /// </summary>
        private void ActivateKitchen()
        {
            isKitchenActive = true;
            _placeHolderSpriteObject.SetActive(!IsKitchenActive);
            _tableSpriteObject.SetActive(IsKitchenActive);
        }

        /// <summary>
        /// Give player upgrade reward
        /// </summary>
        /// <param name="rewardCode"> reward code </param>
        private void SetUpgradeReward(string rewardCode)
        {
            var reward = rewardCode.Split(REWARD_ID_SEPARATOR);
            switch (reward[0]){
                case KitchenUpgradeRewardCurrency.REWARD_TYPE_KEY:
                    new KitchenUpgradeRewardCurrency(reward[1], reward[2]).ReceiveReward();
                    break;
                case KitchenUpgradeRewardLootbox.REWARD_TYPE_KEY:
                    new KitchenUpgradeRewardLootbox(reward[1], reward[2]).ReceiveReward();
                    break;
            }

            StageManager.Instance.SaveStageDataProgression();
        }
    }
}