namespace Project.Gameplay
{
	/// <summary>
	/// Currency data in player wallet
	/// </summary>
	public class Currency
	{
		public enum ID
        {
			Coins,
			Gems
        }

		/// <summary>
		/// Currency id
		/// </summary>
		public ID CurrencyID;

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
