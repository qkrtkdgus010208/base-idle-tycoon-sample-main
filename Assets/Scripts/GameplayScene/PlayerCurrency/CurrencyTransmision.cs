namespace Project.Gameplay
{
	/// <summary>
	/// Currency transmission to player wallet to calculate and update player currencies
	/// </summary>
	public struct CurrencyTransmission
	{
		/// <summary>
		/// Currency id
		/// </summary>
		public Currency.ID CurrencyID;

		/// <summary>
		/// Currency stage
		/// For coin since coin will reset everytime user moving stage
		/// </summary>
		public string StageID;

		/// <summary>
		/// Currency amount
		/// </summary>
		public long Amount;
	}
}
