namespace Project.Gameplay
{
	using System;
	
	[Serializable]
	public struct KitchenLevelData
	{
		/// <summary>
		/// Kitchen level
		/// </summary>
		public int Level;

		/// <summary>
		/// Upgrade cost for this level
		/// </summary>
		public long UpgradeCost;

		/// <summary>
		/// Dish profit in this level
		/// </summary>
		public long DishProfit;

		/// <summary>
		/// Is this level is next phase
		/// </summary>
		public bool IsNextPhase;

		/// <summary>
		/// Is open more table in this level
		/// </summary>
		public bool IsOpenMoreTable;

		/// <summary>
		/// Reward for reach this level (if any)
		/// </summary>
		public string RewardId;
	}
}
