namespace Project.Gameplay
{
	[System.Serializable]
	public struct LootboxSaleData
	{
		/// <summary>
		/// Lootbox ID (back end side)
		/// </summary>
		public string ShopID;

		/// <summary>
		/// Lootbox Contains Items
		/// </summary>
		public string LootboxID;

		/// <summary>
		/// Lootbox Sale Price
		/// </summary>
		public long Price;
	}
}
