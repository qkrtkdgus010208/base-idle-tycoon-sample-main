namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


	public class StageUpgradeUIList : MonoBehaviour
	{
		/// <summary>
		/// stage upgrade icon
		/// </summary>
		[SerializeField] private Image _upgradeIcon;

		/// <summary>
		/// button to upgrade stage
		/// </summary>
		[SerializeField] private Button _upgradeButton;

		/// <summary>
		/// upgrade button image target
		/// </summary>
		[SerializeField] private Image _buttonImage;

		/// <summary>
		/// upgrade stage title text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _upgradeTitle;

		/// <summary>
		/// upgrade stage description text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _upgradeDesc;

		/// <summary>
		/// upgrade price label
		/// </summary>
		[SerializeField] private TextMeshProUGUI _priceText;

		/// <summary>
		/// button prite when purchasable
		/// </summary>
		[SerializeField] private Sprite _purchasableSpriteButton;

		/// <summary>
		/// button sprite when unpurchasable
		/// </summary>
		[SerializeField] private Sprite _unpurchasableSpriteButton;

		/// <summary>
		/// text color when purchasable
		/// </summary>
		[SerializeField] private Color _purchasableTextColor;

		/// <summary>
		/// text color when unpurchasable
		/// </summary>
		[SerializeField] private Color _unpurchasableTextColor;

		/// <summary>
		/// Stage upgrade data that this list ui handler
		/// </summary>
		private StageUpgradeData _upgradeData;

		/// <summary>
		/// purchasable upgrade data state
		/// </summary>
		private bool isPurchasable;

		/// <summary>
		/// purchasable upgrade data state
		/// </summary>
		public bool IsPurchasable => isPurchasable;

		/// <summary>
		/// is stage upgrade ui active state
		/// </summary>
		private bool isStageUpgradeUIActive;


		/// <summary>
		/// Initialize list UI
		/// </summary>
		/// <param name="upgradeData"> stage upgrade data that this list ui handler </param>
		/// <param name="upgradeIcon"> stage upgrade icon </param>
		public void Initialize(StageUpgradeData upgradeData, Sprite upgradeIcon)
		{
			PlayerWallet.OnCurrencyUpdate += OnPlayerCoinsUpdate;

			_upgradeData = upgradeData;

			_upgradeTitle.SetText(_upgradeData.Title);
			_upgradeDesc.SetText(_upgradeData.EffectDescription);
			_priceText.SetText(Utility.StaticCurrencyStringConverison.GetString(_upgradeData.Price));

			_upgradeIcon.sprite = upgradeIcon;
			isPurchasable = StageManager.Instance.GetPlayerCoinAmount() >= _upgradeData.Price;

			_upgradeButton.onClick.AddListener(UpgradeAction);
		}

		/// <summary>
		/// on stage upgrade ui active state change
		/// </summary>
		/// <param name="state"> true: stage upgrade ui active / false: stage upgrade ui inactive </param>
		public void OnStageUpgradeUIActiveStateChange(bool state)
		{
			isStageUpgradeUIActive = state;

			if (state)
				SetElement();
		}

		/// <summary>
		/// on player coins update
		/// </summary>
		/// <param name="id"> currency id </param>
		/// <param name="coinAmount"> currency amount </param>
		private void OnPlayerCoinsUpdate(Currency.ID id, long coinAmount)
		{
			if (id != Currency.ID.Coins)
				return;

			isPurchasable = coinAmount >= _upgradeData.Price;

			if (isStageUpgradeUIActive)
				SetElement();
		}

		/// <summary>
		/// Set UI elements
		/// </summary>
		private void SetElement()
		{
			_buttonImage.sprite = isPurchasable ? _purchasableSpriteButton : _unpurchasableSpriteButton;
			_priceText.color = isPurchasable ? _purchasableTextColor : _unpurchasableTextColor;
		}

		/// <summary>
		/// Upgrade stage action
		/// </summary>
		private void UpgradeAction()
		{
			if (StageManager.Instance.IsUpgradeSuccess(_upgradeData))
			{
				PlayerWallet.OnCurrencyUpdate -= OnPlayerCoinsUpdate;
				
				isPurchasable = false;
				gameObject.SetActive(false);
			}
		}
	}
}
