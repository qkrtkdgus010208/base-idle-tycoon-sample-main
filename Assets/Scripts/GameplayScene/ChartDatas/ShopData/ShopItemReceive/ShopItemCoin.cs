namespace Project.Gameplay
{
	public class ShopItemCoin : AbstractShopItemReceive
	{
		/// <summary>
		/// Coins item code id
		/// </summary>
		public const string STR_ITEM_KEY = "BIPC";

		/// <summary>
		/// Coins amount that will player receive
		/// </summary>
		private long _coinAmount;


		public ShopItemCoin(string coinAmount)
        {
			_coinAmount = long.Parse(coinAmount);
        }

		/// <summary>
		/// Player receive item
		/// </summary>
		public override void ReceiveItem()
		{
			StageManager.Instance.PlayerCoinIncome(_coinAmount, true);
		}
	}
}