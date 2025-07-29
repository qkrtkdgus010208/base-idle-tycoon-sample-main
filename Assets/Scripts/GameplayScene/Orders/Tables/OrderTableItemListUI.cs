namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;


	public class OrderTableItemListUI : MonoBehaviour
	{
		/// <summary>
		/// Image for order item icon
		/// </summary>
		[SerializeField] private Image _orderIconImage;

		/// <summary>
		/// order count text
		/// </summary>
		[SerializeField] private TextMeshProUGUI _orderCounterText;

		/// <summary>
		/// Set ui element
		/// </summary>
		/// <param name="icon"> order icon </param>
		/// <param name="amount"> order amount </param>
		public void SetOrderUI(Sprite icon, int amount)
        {
			_orderIconImage.sprite = icon;
			_orderCounterText.SetText(amount.ToString());
        }

		/// <summary>
		/// Set active item in list
		/// </summary>
		/// <param name="state"> true: set active / false: deactive </param>
		public void SetActive(bool state)
        {
			gameObject.SetActive(state);
        }
	}
}
