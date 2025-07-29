namespace Project.Gameplay
{
	public class ShopItemLootBox : AbstractShopItemReceive
	{
		/// <summary>
		/// Lootbox item code id
		/// </summary>
		public const string STR_ITEM_KEY = "BILB";

		/// <summary>
		/// Lootbox Id that will player receive
		/// </summary>
		private string _lootBoxID;

		/// <summary>
		/// Lootbox amount that will player receive
		/// </summary>
		private int _amount;

		public ShopItemLootBox() { }

		public ShopItemLootBox(string lootBoxID, string amount)
        {
			_lootBoxID = lootBoxID; // Parse from database code
			_amount = int.Parse(amount); // Parse from database code
		}

		/// <summary>
		/// Set lootbox data
		/// </summary>
		/// <param name="lootBoxID"> Lootbox id that will player receive </param>
		/// <param name="amount"> Lootbox amount that will player receive </param>
		public void SetItem(string lootBoxID, int amount)
		{
			_lootBoxID = lootBoxID;
			_amount = amount;
		}

		/// <summary>
		/// Player receive item
		/// </summary>
		public override void ReceiveItem()
		{
			for (int i = 0; i < _amount; i++)
				LootBoxManager.OpenLootBox?.Invoke(_lootBoxID);
		}
	}
}