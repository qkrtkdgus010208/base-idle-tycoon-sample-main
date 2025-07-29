namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


	public class MovingStageUI : MonoBehaviour
	{
        /// <summary>
        /// stage load data container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _loadData;

        /// <summary>
        /// stage default data container
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stageDatas;

        /// <summary>
        /// kitchen data container
        /// </summary>
        [SerializeField] private SO_BatchKitchenLevelDataCollection _kitchensData;

        /// <summary>
        /// last stage id
        /// </summary>
        [SerializeField] private string lastStageID;
		
        /// <summary>
        /// moving stage ui
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// moving stage title
        /// </summary>
        [SerializeField] private GameObject _titleText;

        /// <summary>
        /// requirement text message
        /// </summary>
        [SerializeField] private GameObject _movingStageRequirementMessage;

        /// <summary>
        /// last stage text message
        /// </summary>
        [SerializeField] private GameObject _lastStageMessage;

        /// <summary>
        /// indicator to indicate is player available to move right now
        /// </summary>
        [SerializeField] private GameObject _availableToMoveIndicator;

        /// <summary>
        /// button to move to next stage
        /// </summary>
		[SerializeField] private Button _moveButton;

        /// <summary>
        /// button to open moving stage ui
        /// </summary>
        [SerializeField] private Button _openUIButton;

        /// <summary>
        /// button to close moving stage ui
        /// </summary>
        [SerializeField] private Button _closedUIButton;

        /// <summary>
        /// moving stage price label
        /// </summary>
        [SerializeField] private TextMeshProUGUI _priceText;

        /// <summary>
        /// list of kitchen in current played stage
        /// </summary>
        private Kitchenware[] _kitchensInCurrentStage;

        /// <summary>
        /// moving to next stage price
        /// </summary>
        private long currentFinishStagePrice;

        /// <summary>
        /// is last stage state
        /// </summary>
        private bool isLastStage;

        /// <summary>
        /// is ui active state
        /// </summary>
        private bool isUIActive;


        private void Awake()
        {
            PlayerWallet.OnCurrencyUpdate += OnPlayerCoinsUpdate;

            _moveButton.onClick.AddListener(MovingToNextStage);

            _openUIButton.onClick.AddListener(() => SetActiveUI(true));
            _closedUIButton.onClick.AddListener(() => SetActiveUI(false));
        }

        private void Start()
        {
            var stageData = _stageDatas.GetStageDataById(_loadData.CurrentStageID);

            _priceText.SetText(
                Utility.StaticCurrencyStringConverison.GetString(stageData.FinishStagePrice)
                );

            isLastStage = string.Equals(_loadData.CurrentStageID, lastStageID);
            currentFinishStagePrice = _stageDatas.GetStageDataById(_loadData.CurrentStageID).FinishStagePrice;
            _kitchensInCurrentStage = _loadData.CurrentStageKitchensData.Kitchens;

            _moveButton.gameObject.SetActive(!isLastStage);
            _titleText.SetActive(!isLastStage);
            _movingStageRequirementMessage.SetActive(!isLastStage);

            _lastStageMessage.SetActive(isLastStage);

            if (isLastStage && PlayerWallet.GetCurrentCurrency(Currency.ID.Coins, _loadData.CurrentStageID) == _stageDatas.GetStageDataById(lastStageID).StartingCoin)
                SetActiveUI(true);

            SetUIElement(IsAllRequirementsMet(), IsEnoughCoinsMovingStage());
        }

        private void OnDestroy()
        {
            PlayerWallet.OnCurrencyUpdate -= OnPlayerCoinsUpdate;
        }

        /// <summary>
        /// Set active moving stage ui
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveUI(bool state)
        {
            isUIActive = state;
            _ui.SetActive(state);

            if (state)
                SetUIElement(IsAllRequirementsMet(), IsEnoughCoinsMovingStage());
        }

        /// <summary>
        /// on player coins update
        /// Set elements when available to moving to next stage
        /// </summary>
        /// <param name="currencyID"> currency id </param>
        /// <param name="playerCoin"> currency amount </param>
        private void OnPlayerCoinsUpdate(Currency.ID currencyID, long playerCoin)
        {
            if (currencyID != Currency.ID.Coins)
                return;

            bool isEnoughCoins = playerCoin >= currentFinishStagePrice;
            bool isRequirementsValid = IsAllRequirementsMet();


            SetUIElement(isRequirementsValid, isEnoughCoins);
        }

        /// <summary>
        /// Check if coins is enough to moving to next stage
        /// </summary>
        /// <returns> true: enough coins / false: not enough coins </returns>
        private bool IsEnoughCoinsMovingStage()
            => StageManager.Instance.GetPlayerCoinAmount() >= currentFinishStagePrice;

        /// <summary>
        /// Checks whether all kitchen upgrade requirements are fulfilled for the current stage.
        /// </summary>
        /// <returns>
        /// true: if all kitchens in the current stage have reached their maximum level
        /// </returns>
        private bool IsAllRequirementsMet()
        {
            var stageData = SaveData.UserStageDataManager.Instance.GetStageDataByID(_loadData.CurrentStageID);
            if (stageData == null)
                return false;

            var userKitchensDatas = stageData.KitchenDatas;
            if (userKitchensDatas == null)
                return false;

            // Iterate through all kitchens in the current stage
            foreach (var kitchen in _kitchensInCurrentStage)
            {
                // Find the matching user kitchen data
                var userKitchenData = userKitchensDatas.Find(x => string.Equals(x.KitchenDataID, kitchen.DishID));

                if (userKitchenData == null)
                    return false;

                if (userKitchenData.KitchenDataLevel < _kitchensData.GetKitchenDataByDishID(kitchen.DishID).MaxLevel)
                    return false;
            }

            // All kitchens meet the requirement
            return true;
        }

        /// <summary>
        /// Set moving stage ui elements
        /// </summary>
        /// <param name="isRequirementValid"> Requirement to move to next stage valid </param>
        /// <param name="isEnoughCoins"> coins enough to move to next stage </param>
        private void SetUIElement(bool isRequirementValid, bool isEnoughCoins)
        {
            if (isLastStage)
                return;

            if (isUIActive)
            {
                _movingStageRequirementMessage.SetActive(!isRequirementValid);
                _moveButton.interactable = isRequirementValid && isEnoughCoins;
            }
            else
                _availableToMoveIndicator.SetActive(isRequirementValid && isEnoughCoins);
        }

        /// <summary>
        /// moving to next stage button action
        /// </summary>
        private void MovingToNextStage()
        {
            if (StageManager.Instance.IsMovingStageSuccess(
                    _stageDatas.GetStageDataById(_loadData.CurrentStageID),
                    _stageDatas.GetStageDataById(_loadData.CurrentStageID, +1)
                    ))
                SetActiveUI(false);
        }
    }
}
