namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Events;

    public class ShopSaleUIList : MonoBehaviour
	{
        /// <summary>
        /// image for item in sale icon
        /// </summary>
        [SerializeField] private Image _iconImage;

        /// <summary>
        /// item in sale title text output
        /// </summary>
		[SerializeField] private TextMeshProUGUI _titleText;

        /// <summary>
        /// item in sale price label
        /// </summary>
		[SerializeField] private TextMeshProUGUI _priceText;

        /// <summary>
        /// button to open sale item info to purchase
        /// </summary>
		[SerializeField] private Button _purchaseButton;

        /// <summary>
        /// indicator when item sold out
        /// </summary>
        [SerializeField] private GameObject _soldIndicator;
        
        /// <summary>
        /// Initailize shop ui list
        /// </summary>
        /// <param name="icon"> sale item icon </param>
        /// <param name="title"> sale item title </param>
        /// <param name="price"> sale item price </param>
        /// <param name="onButtonPurchase"> on sale item button clicked action </param>
		public void Initialize(Sprite icon, string title, string price, UnityAction onButtonPurchase)
        {
            _iconImage.sprite = icon;

            _titleText.SetText(title);
            _priceText.SetText(price);
            _purchaseButton.onClick.AddListener(onButtonPurchase);
        }

        /// <summary>
        /// Set ui elements
        /// </summary>
        /// <param name="isAvailable"> is available to purchase </param>
        public void SetElementUI(bool isAvailable)
        {
            _purchaseButton.interactable = isAvailable;
            
            _priceText.gameObject.SetActive(isAvailable);
            _soldIndicator.SetActive(!isAvailable);
        }
	}
}
