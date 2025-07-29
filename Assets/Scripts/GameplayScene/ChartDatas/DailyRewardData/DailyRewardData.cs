namespace Project.Gameplay
{
	[System.Serializable]
	public struct DailyRewardData
	{
		/// <summary>
		/// Day idx in daily reward list
		/// </summary>
		public int DayIdx;

		/// <summary>
		/// Reward data
		/// </summary>
		public string RewardData;

		/// <summary>
		/// Reward item (reward data after parse)
		/// </summary>
		public AbstractDailyRewardReceiveItem RewardItem;
	}
}
