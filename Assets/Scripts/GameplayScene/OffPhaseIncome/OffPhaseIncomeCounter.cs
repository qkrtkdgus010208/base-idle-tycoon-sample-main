namespace Project.Gameplay
{
    using SaveData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


    public class OffPhaseIncomeCounter : MonoBehaviour
    {
        /// <summary>
        /// Minimum user off time long
        /// </summary>
        private const int MIN_OFF_PHASE_INCOME_TIME_IN_MINUTE = 1;

        /// <summary>
        /// Maximum time user can get from off phase income
        /// </summary>
        private const int MAX_OFF_PHASE_INCOME_TIME_IN_HOURS = 5;

        /// <summary>
        /// Income multiply (120%)
        /// </summary>
        private const float INCOME_MULTIPLY = 120 / 100;

        /// <summary>
        /// Minute to hours convesion
        /// </summary>
        private const int MINUTE_TO_HOURS = 60;

        /// <summary>
        /// Hours symbol
        /// </summary>
        private const char HOURS_TIME_SYMBOL = 'h';

        /// <summary>
        /// Minute symbol
        /// </summary>
        private const char MINUTE_TIME_SYMBOL = 'm';

        /// <summary>
        /// load stage data container
        /// </summary>
        [SerializeField] private SO_StageLoadData _dataLoader;

        /// <summary>
        /// kitchen data container
        /// </summary>
        [SerializeField] private SO_BatchKitchenLevelDataCollection _kitchensDataCollection;

        /// <summary>
        /// off phase income UI
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// off phase income coins amount
        /// </summary>
        [SerializeField] private TextMeshProUGUI _txtCoinsAmount;

        /// <summary>
        /// off phase timer text
        /// </summary>
        [SerializeField] private TextMeshProUGUI _txtTimer;

        /// <summary>
        /// timer bar to max off phase income time
        /// </summary>
        [SerializeField] private Slider _sldTimerBar;

        /// <summary>
        /// button to collect off phase income
        /// </summary>
        [SerializeField] private Button _collectButton;

        /// <summary>
        /// button to collect double off phase income
        /// </summary>
        [SerializeField] private Button _doubleCollectButton;

        /// <summary>
        /// Off phase income coins amount
        /// </summary>
        private long coinsAmount;


        private void Awake()
        {
            _collectButton.onClick.AddListener(CollectIncome);
            _doubleCollectButton.onClick.AddListener(CollectDoubleIncome);
        }

        private void Start()
        {
            var stageData = UserStageDataManager.Instance.GetStageDataByID(_dataLoader.CurrentStageID);
            if (stageData == null)
                return;

            long kitchenIncome = 0;
            foreach (var kitchen in stageData.KitchenDatas)
                kitchenIncome += _kitchensDataCollection.GetKitchenDataByDishID(kitchen.KitchenDataID).GetLevelData(kitchen.KitchenDataLevel).DishProfit;

            DateTime lastLogout = _dataLoader.LastPlayerLogoutDate;
            DateTime currentTime = BackndServer.BackndServerTime.GetServerTime();

            float totalTime = Mathf.Min(
                (float)currentTime.Subtract(lastLogout).TotalMinutes,
                MAX_OFF_PHASE_INCOME_TIME_IN_HOURS * MINUTE_TO_HOURS);

            coinsAmount = Mathf.RoundToInt(kitchenIncome * totalTime * INCOME_MULTIPLY);

            if (totalTime > MIN_OFF_PHASE_INCOME_TIME_IN_MINUTE && coinsAmount > 0)
                ShowIncomeUI(Mathf.FloorToInt(totalTime));
        }

        /// <summary>
        /// Showing off phase income UI
        /// </summary>
        /// <param name="totalTime"> player off time long </param>
        private void ShowIncomeUI(int totalTime)
        {
            _txtCoinsAmount.SetText(Utility.StaticCurrencyStringConverison.GetString(coinsAmount));

            int parseTime = totalTime;
            char timeSymbol = MINUTE_TIME_SYMBOL;
            if (parseTime > MINUTE_TO_HOURS)
            {
                parseTime /= MINUTE_TO_HOURS;
                timeSymbol = HOURS_TIME_SYMBOL;
            }

            _txtTimer.SetText(parseTime.ToString() + timeSymbol + "/" + MAX_OFF_PHASE_INCOME_TIME_IN_HOURS + HOURS_TIME_SYMBOL);
            
            _sldTimerBar.maxValue = MAX_OFF_PHASE_INCOME_TIME_IN_HOURS * MINUTE_TO_HOURS;
            _sldTimerBar.SetValueWithoutNotify(totalTime);

            _ui.SetActive(true);
        }

        /// <summary>
        /// Collect income
        /// </summary>
        private void CollectIncome()
        {
            StageManager.Instance.PlayerCoinIncome(coinsAmount);
            _ui.SetActive(false);
        }

        /// <summary>
        /// Collect double income
        /// ** Ads not implement yet **
        /// </summary>
        private void CollectDoubleIncome()
        {
            StageManager.Instance.PlayerCoinIncome(coinsAmount * 2);
            _ui.SetActive(false);
        }
    }
}