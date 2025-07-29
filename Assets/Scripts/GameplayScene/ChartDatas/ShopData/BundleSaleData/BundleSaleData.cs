namespace Project.Gameplay
{
	[System.Serializable]
	public struct BundleSaleData
	{
		/// <summary>
		/// Bundle ID (back end side)
		/// </summary>
		public string BundleID;

		/// <summary>
		/// Bundle Name Output / Display Name
		/// </summary>
		public string BundleName;
		
		/// <summary>
		/// Bundle Contains Items
		/// </summary>
		public string ContainsItems;

		/// <summary>
		/// List of Items Object that will player receive
		/// </summary>
		public AbstractShopItemReceive[] ContainsItemsObjects;
	}
}
