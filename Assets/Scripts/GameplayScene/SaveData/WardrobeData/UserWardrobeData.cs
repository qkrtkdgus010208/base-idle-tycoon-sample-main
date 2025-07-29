namespace Project.Gameplay.SaveData
{
	public class UserWardrobeData
	{
		/// <summary>
		/// backnd's data id
		/// </summary>
		public string inDate;

		/// <summary>
		/// dress primary key
		/// </summary>
		public string DressID;

		/// <summary>
		/// total xp collected
		/// will convert to level
		/// </summary>
		public int DressTotalXP;

		/// <summary>
		/// data update state
		/// to indicate data need to save
		/// </summary>
		public bool IsDataChange;
	}
}
