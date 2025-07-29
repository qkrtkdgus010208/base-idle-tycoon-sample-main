namespace Project.Gameplay
{
    using UnityEngine;
    using TMPro;


	public class PlayerCurrencyUI : MonoBehaviour
	{
        /// <summary>
        /// Stage load data container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _dataLoader;

        /// <summary>
        /// Currency id that this ui handle to show
        /// </summary>
        [SerializeField] private Currency.ID _currencyID;

        /// <summary>
        /// Currency amount text
        /// </summary>
		[SerializeField] private TextMeshProUGUI _text;

        private void Awake()
        {
            PlayerWallet.OnCurrencyUpdate += UpdateUI;
        }

        private void OnDestroy()
        {
            PlayerWallet.OnCurrencyUpdate -= UpdateUI;
        }

        private void Start()
        {
            _text.SetText(
                Utility.StaticCurrencyStringConverison.GetString(PlayerWallet.GetCurrentCurrency(_currencyID, _dataLoader.CurrentStageID))
                );
        }

        /// <summary>
        /// Update currency amount
        /// </summary>
        /// <param name="id"> target currency id </param>
        /// <param name="currencyAmount"> currency amount </param>
        private void UpdateUI(Currency.ID id, long currencyAmount)
        {
            if (id == _currencyID)
                _text.SetText(Utility.StaticCurrencyStringConverison.GetString(currencyAmount));
        }
    }
}
