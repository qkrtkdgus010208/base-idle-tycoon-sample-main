namespace Project.Gameplay
{
	[System.Serializable]
	public struct GemsSaleData
	{
		/// <summary>
		/// Gems ID (back end side)
		/// </summary>
		public string GemsSaleID;

		/// <summary>
		/// Gems Name Output / Display Name
		/// </summary>
		public string GemsSaleName;

		/// <summary>
		/// Gems Contains Items
		/// </summary>
		public long Amount;
	}
}
