namespace Project.Gameplay.SaveData
{
	[System.Serializable]
	public struct UserStageData
	{
		/// <summary>
		/// backnd's data id
		/// </summary>
		public string inDate;

		/// <summary>
		/// primary key
		/// </summary>
		public string StageID;

		/// <summary>
		/// Player coin in the stage
		/// </summary>
		public long PlayerCoin;

		/// <summary>
		/// stage upgrade level
		/// </summary>
		public string UpgradeLevel;

		/// <summary>
		/// kitchen station datas
		/// </summary>
		public string KitchenDatas;
	}
}
