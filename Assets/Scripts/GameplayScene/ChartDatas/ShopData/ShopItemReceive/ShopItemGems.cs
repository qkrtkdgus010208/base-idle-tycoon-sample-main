namespace Project.Gameplay
{
	public class ShopItemGems : AbstractShopItemReceive
	{
		/// <summary>
		/// Gems item code id
		/// </summary>
		public const string STR_ITEM_KEY = "BIPG";

		/// <summary>
		/// Gems amount that will player receive
		/// </summary>
		private long _gemsAmount;

		public ShopItemGems() { }

		public ShopItemGems(string gemsAmount)
        {
			_gemsAmount = long.Parse(gemsAmount); // parse from database code
        }

		/// <summary>
		/// Set gems amount that will player receive
		/// </summary>
		/// <param name="gemsAmount"> Gems amount that will player receive </param>
		public void SetGemsAmount(long gemsAmount)
        {
			_gemsAmount = gemsAmount;
        }

		/// <summary>
		/// Player receive item
		/// </summary>
		public override void ReceiveItem()
		{
			PlayerWallet.PlayerIncome(new CurrencyTransmission() { CurrencyID = Currency.ID.Gems, Amount = _gemsAmount }, true);
		}
	}
}