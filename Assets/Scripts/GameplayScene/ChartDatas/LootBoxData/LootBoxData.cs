namespace Project.Gameplay
{
	[System.Serializable]
	public struct LootBoxData
	{
		/// <summary>
		/// Lootbox description text
		/// {0}: minimal item can include in lootbox
		/// {1}: maximal item can include in lootbox
		/// </summary>
		private const string DESCRIPTION = "Contains {0} to {1} items";
		
		/// <summary>
		/// Loot Box ID (back end side)
		/// </summary>
		public string LootBoxID;

		/// <summary>
		/// Loot Box Name output name
		/// </summary>
		public string LootBoxName;

		/// <summary>
		/// Minimal item can include in lootbox
		/// </summary>
		public int MinItem;

		/// <summary>
		/// Maximal item can include in lootbox
		/// </summary>
		public int MaxItem;

		/// <summary>
		/// Lootbox description text
		/// </summary>
		private string _description;

		/// <summary>
		/// Lootbox description text
		/// </summary>
		public string Description
		{
			get
			{
				if (string.IsNullOrEmpty(_description)) 
					_description = string.Format(DESCRIPTION, MinItem, MaxItem);

				return _description;
			}
		}
	}
}
