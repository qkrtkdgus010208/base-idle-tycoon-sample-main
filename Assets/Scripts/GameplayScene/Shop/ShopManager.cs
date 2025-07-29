namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


	public class ShopManager : MonoBehaviour
	{
        /// <summary>
        /// IAP data container
        /// </summary>
        [SerializeField] private SOIAPCollections _iapCollections;

        /// <summary>
        /// bundle sale data container
        /// </summary>
        [SerializeField] private SO_BatchBundleSaleData _bundleSaleData;

        /// <summary>
        /// gem sale data container
        /// </summary>
        [SerializeField] private SO_BatchGemsSaleData _gemsSaleData;

        /// <summary>
        /// loobox sale data container
        /// </summary>
        [SerializeField] private SO_BatchLootboxSaleData _lootboxSaleData;

        /// <summary>
        /// lootbox data container
        /// </summary>
        [SerializeField] private SO_BatchLootBoxData _lootboxData;

        /// <summary>
        /// shop ui
        /// </summary>
		[SerializeField] private GameObject _ui;

        /// <summary>
        /// open shop button
        /// </summary>
		[SerializeField] private Button _openShopButton;

        /// <summary>
        /// close shop button
        /// </summary>
		[SerializeField] private Button _closeShopButton;

        /// <summary>
        /// coin icon sprite
        /// </summary>
        [SerializeField] private Sprite _coinPriceIcons;

        /// <summary>
        /// gems icon sprite
        /// </summary>
        [SerializeField] private Sprite _gemsPriceIcons;

        /// <summary>
        /// ads icon sprite
        /// </summary>
        [SerializeField] private Sprite _adsPriceIcons;

        /// <summary>
        /// template ui for bundle sale list
        /// </summary>
        [SerializeField] private ShopSaleUIList _bundleSaleTemplateUI;

        /// <summary>
        /// parent transform for list of bundle sale
        /// </summary>
        [SerializeField] private Transform _bundleSaleUIListParent;
        
        /// <summary>
        /// template ui for gems sale list
        /// </summary>
        [SerializeField] private ShopSaleUIList _gemsSaleTemplateUI;

        /// <summary>
        /// parent transform for list of gems sale
        /// </summary>
        [SerializeField] private Transform _gemsSaleUIListParent;

        /// <summary>
        /// template ui for lootbox sale list
        /// </summary>
        [SerializeField] private ShopSaleUIList _lootboxSaleTemplateUI;

        /// <summary>
        /// parent transform for list of lootbox sale
        /// </summary>
        [SerializeField] private Transform _lootboxSaleUIListParent;

        
        [Space(20)]
        [Header("Purchase Panel Content")]
        /// <summary>
        /// purchase content panel
        /// panel that will show up, after player click
        /// sale item in shop
        /// </summary>
        [SerializeField] private GameObject _purchaseContentPanel;

        /// <summary>
        /// Image for icon item
        /// </summary>
        [SerializeField] private Image _purchaseIconImage;

        /// <summary>
        /// Item price currency icon
        /// </summary>
        [SerializeField] private Image _priceCurrencyIconImage;

        /// <summary>
        /// Item title output text 
        /// </summary>
        [SerializeField] private TextMeshProUGUI _purchaseContentTitleText;
        
        /// <summary>
        /// Item description output text 
        /// </summary>
        [SerializeField] private TextMeshProUGUI _purchaseContentDescText;

        /// <summary>
        /// Item price price
        /// </summary>
        [SerializeField] private TextMeshProUGUI _purchasePriceText;

        /// <summary>
        /// confirm purchasing button
        /// </summary>
        [SerializeField] private Button _confirmPurchaseButton;

        /// <summary>
        /// close or cancel purchase button
        /// </summary>
        [SerializeField] private Button _closePurchaseButton;

        /// <summary>
        /// Open shop UI events
        /// </summary>
        public static Action OpenShopUI;

        /// <summary>
        /// temporary object obtain gems after player purchasing item gems sale;
        /// </summary>
        private ShopItemGems _singlePurchasedGemsTemp = new ShopItemGems();

        /// <summary>
        /// temporary object obtain lootbox after player purchasing item lootbox sale;
        /// </summary>
        private ShopItemLootBox _singlePurchasedLootboxTemp = new ShopItemLootBox();

        private void Awake()
        {
            OpenShopUI = () => SetActiveUI(true);

            _openShopButton.onClick.AddListener(() => SetActiveUI(true));
            _closeShopButton.onClick.AddListener(() => SetActiveUI(false));

            _closePurchaseButton.onClick.AddListener(() => _purchaseContentPanel.SetActive(false));
        }

        private void Start()
        {
            InitializeBundleSale();
            InitializeGemsSale();
            InitializeLootboxSale();
        }

        /// <summary>
        /// Initialize list of bundle sale 
        /// </summary>
        private void InitializeBundleSale()
        {
            foreach (var data in _bundleSaleData.BundleDatas)
            {
                var iapData = _iapCollections.GetIAPItemByID(data.BundleID);
                var listUI = Instantiate(_bundleSaleTemplateUI, _bundleSaleUIListParent);

                listUI.Initialize(
                    iapData.IAPIcon,
                    iapData.InAppPurchaseTitle,
                    iapData.InAppPurchasePricesString,
                    () =>
                    {
                        OpenPurchasePanel(
                            iapData.InAppPurchaseTitle,
                            iapData.InAppPurchaseDescription,
                            iapData.InAppPurchasePricesString,
                            iapData.IAPIcon,
                            null,
                            () => PurchaseIAP(iapData.InAppPurchaseID,
                                (successPurchaseID) =>
                                {
                                    OnPurchaseSuccess(successPurchaseID, data.BundleID, data.ContainsItemsObjects);
                                    listUI.SetElementUI(false);
                                }));
                    });

                listUI.SetElementUI(!iapData.InAppPurchaseIsOwned);
            }
        }

        /// <summary>
        /// Initialize list of gems sale 
        /// </summary>
        private void InitializeGemsSale()
        {
            foreach (var data in _gemsSaleData.GemsSaleDatas)
            {
                var iapData = _iapCollections.GetIAPItemByID(data.GemsSaleID); ;

                if (iapData == null)
                    continue;

                var listUI = Instantiate(_gemsSaleTemplateUI, _gemsSaleUIListParent);
                listUI.Initialize(
                    iapData.IAPIcon,
                    iapData.InAppPurchaseTitle,
                    iapData.InAppPurchasePricesString,
                    () => OpenPurchasePanel(
                        iapData.InAppPurchaseTitle,
                        iapData.InAppPurchaseDescription,
                        iapData.InAppPurchasePricesString,
                        iapData.IAPIcon,
                        null,
                        () => PurchaseIAP(iapData.InAppPurchaseID,
                        (iapSuccessID) =>
                        {
                            _singlePurchasedGemsTemp.SetGemsAmount(data.Amount);
                            OnPurchaseSuccess(iapSuccessID, data.GemsSaleID, _singlePurchasedGemsTemp);
                        }))
                    );
            }
        }

        /// <summary>
        /// Initialize list of lootbox sale 
        /// </summary>
        private void InitializeLootboxSale()
        {
            foreach (var data in _lootboxSaleData.LootboxSaleDatas)
            {
                var lootbox = _lootboxData.GetLootBoxDataByID(data.LootboxID);
                var icon = lootbox.CloseSpriteObject;
                var listUI = Instantiate(_lootboxSaleTemplateUI, _lootboxSaleUIListParent);

                listUI.Initialize(
                    icon,
                    lootbox.LootboxData.LootBoxName,
                    data.Price.ToString(),
                    () => OpenPurchasePanel(
                        lootbox.LootboxData.LootBoxName,
                        lootbox.LootboxData.Description,
                        data.Price.ToString(),
                        icon,
                        _gemsPriceIcons,
                        () =>
                        {
                            _singlePurchasedLootboxTemp.SetItem(data.LootboxID, 1);
                            PurchaseNormalItems(
                                new CurrencyTransmission() 
                                {
                                    CurrencyID = Currency.ID.Gems,
                                    Amount = data.Price 
                                }, 
                                _singlePurchasedLootboxTemp);
                        })
                    );
            }
        }

        /// <summary>
        /// Open purchase content panel
        /// </summary>
        /// <param name="title"> sale item title </param>
        /// <param name="desc"> sale item description </param>
        /// <param name="price"> sale item price </param>
        /// <param name="icon"> sale item icon </param>
        /// <param name="priceCurrencyIcon"> sale item currency icon </param>
        /// <param name="onPurchase"> callback when confirm to purchase </param>
        private void OpenPurchasePanel(
            string title,
            string desc,
            string price,
            Sprite icon,
            Sprite priceCurrencyIcon,
            Action onPurchase)
        {
            _purchaseIconImage.sprite = icon;

            _purchaseContentTitleText.SetText(title);
            _purchaseContentDescText.SetText(desc);
            _purchasePriceText.SetText(price);

            if (priceCurrencyIcon != null)
            {
                _priceCurrencyIconImage.sprite = priceCurrencyIcon;
                _priceCurrencyIconImage.gameObject.SetActive(true);
            }
            else
                _priceCurrencyIconImage.gameObject.SetActive(false);

            _confirmPurchaseButton.onClick.RemoveAllListeners();
            _confirmPurchaseButton.onClick.AddListener(() => onPurchase?.Invoke());

            _purchaseContentPanel.SetActive(true);
        }

        /// <summary>
        /// IAP purchasing action
        /// </summary>
        /// <param name="pruchaseID"> target iap id </param>
        /// <param name="onPurchaseSuccess"> callback when successful purchase </param>
        private void PurchaseIAP(string pruchaseID, Action<string> onPurchaseSuccess)
        {
            StaticEventIAP.onPurchaseSuccess = onPurchaseSuccess;
            StaticEventIAP.SetPurchaseIAPItem?.Invoke(pruchaseID);
        }

        /// <summary>
        /// Normal item purchasing action
        /// </summary>
        /// <param name="price"> item price </param>
        /// <param name="receiveItems"> item will receive after successful purchase </param>
        private void PurchaseNormalItems(CurrencyTransmission price, AbstractShopItemReceive receiveItems)
        {
            if (!PlayerWallet.PlayerPayment(price))
            {
                FloatingTextPool.Instance.ShowFloatingText(
                    Utility.StaticConstantDictionary.NOT_ENOUGH_RESOURCES_MESSAGE,
                    Input.mousePosition,
                    FloatingTextObj.Position_State.Screen,
                    FloatingTextObj.Text_State.Invalid
                    );

                return;
            }

            receiveItems.ReceiveItem();
            _purchaseContentPanel.SetActive(false); 
        }

        /// <summary>
        /// After IAP purchasing success for bundles item
        /// </summary>
        /// <param name="successItemID"> iap item id that successful purchased </param>
        /// <param name="currentItemID"> current purchase item id </param>
        /// <param name="receiveItems"> list of item to receive </param>
        private void OnPurchaseSuccess(string successItemID, string currentItemID, AbstractShopItemReceive[] receiveItems)
        {
            if (!string.Equals(successItemID, currentItemID))
                return;

            foreach (var item in receiveItems)
                item.ReceiveItem();

            _purchaseContentPanel.SetActive(false);
        }

        /// <summary>
        /// After IAP purchasing success for single item
        /// </summary>
        /// <param name="successItemID"> iap item id that successful purchased </param>
        /// <param name="currentItemID"> current purchase item id </param>
        /// <param name="receiveItems"> item to receive </param>
        private void OnPurchaseSuccess(string successItemID, string currentItemID, AbstractShopItemReceive receiveItems)
        {
            if (!string.Equals(successItemID, currentItemID))
                return;

            receiveItems.ReceiveItem();
            _purchaseContentPanel.SetActive(false);
        }

        /// <summary>
        /// Set active ui shop state
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveUI(bool state)
        {
#if UNITY_EDITOR
            _ui.SetActive(state);
#endif
        }
	}
}
