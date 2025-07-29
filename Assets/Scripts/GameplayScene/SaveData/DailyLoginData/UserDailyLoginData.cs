namespace Project.Gameplay.SaveData
{
	public class UserDailyLoginData
	{
		/// <summary>
		/// backnd's data id
		/// </summary>
		public string inDate;

		/// <summary>
		/// player daily reward index
		/// daily reward data
		/// </summary>
		public int DailyRewardIdx;

		/// <summary>
		/// player last claim daily reward date
		/// daily reward data
		/// </summary>
		public string LastClaimedDailyRewardDate;

		/// <summary>
		/// player total login count
		/// </summary>
		public int TotalLoginCount;

		/// <summary>
		/// player current login streak
		/// reset to 0 when range between player login > 1 day
		/// </summary>
		public int LoginStreakCount;
	}
}
