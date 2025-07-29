namespace Project.Gameplay
{
	using System;

	[Serializable]
	public struct StageDefaultData
	{
		/// <summary>
		/// Stage id
		/// </summary>
		public string StageID;

		/// <summary>
		/// Stage name
		/// </summary>
		public string StageName;

		/// <summary>
		/// Starting coin
		/// When player reach the stage player coin will reset into this
		/// </summary>
		public long StartingCoin;

		/// <summary>
		/// Price to finish this stage
		/// and move to the next stage
		/// </summary>
		public long FinishStagePrice;

		/// <summary>
		/// Customer spawn time rate
		/// </summary>
		public float CustomerSpawnTime;

		/// <summary>
		/// Maximal number of order that customer can order
		/// </summary>
		public int MaxCustomerOrderCount;
		
		/// <summary>
		/// Maximal order / dish variant that customer can order
		/// </summary>
		public int MaxCustomerOrderVariant;

		/// <summary>
		/// Order table that already opened from start of this stage
		/// </summary>
		public int StartingOrderTableCount;

		/// <summary>
		/// Number of helper that already in kitchen from start of this stage
		/// </summary>
		public int StartingStaffHelperCount;

		/// <summary>
		/// Camera zoom size of this stage
		/// </summary>
		public int CameraZoomSize;
	}
}
