namespace Project.Gameplay
{
	[System.Serializable]
	public struct CustomerData
	{
		/// <summary>
		/// Available Customer ID
		/// </summary>
		public enum Customer_ID
        {
			CustomerA,
			CustomerB,
			CustomerC,
			CustomerD,
			Washington,
			TonyGawk,
			Zeus,
			Vangogh,
			SuperHumanman,
		}

		/// <summary>
		/// Customer id
		/// </summary>
		public Customer_ID CustomerID;

		/// <summary>
		/// Customer name
		/// </summary>
		public string CustomerName;

		/// <summary>
		/// Customer description
		/// </summary>
		public string CustomerDescription;

		/// <summary>
		/// Cutomer effect description
		/// </summary>
		public string EffectDescription;

		/// <summary>
		/// Customer effect data in string code
		/// </summary>
		public string EffectData;
	}
}
