namespace Project.Gameplay
{
	using BackndServer;
	using System;
	using System.Collections.Generic;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Leaderboard/Leaderboard Batch Data")]
	public class SO_LeaderboardData : ScriptableObject
	{
		/// <summary>
		/// List of leaderboard table
		/// </summary>
		private List<BackEnd.Leaderboard.LeaderboardTableItem> _leaderboardTableList;

		/// <summary>
		/// List of leaderboard table
		/// </summary>
		public List<BackEnd.Leaderboard.LeaderboardTableItem> LeaderboardTableList => _leaderboardTableList;

		/// <summary>
		/// Load all leaderboard table in backnd console
		/// </summary>
		public void LoadData()
        {
			BackndLeaderboard.LoadLeaderBoard((leaderboardList) 
				=> _leaderboardTableList = leaderboardList);
        }

		/// <summary>
		/// Get specific leaderboard data
		/// </summary>
		/// <param name="leaderboardID"> target leaderboard id </param>
		/// <param name="onGetLeaderboardData"> callback after get leaderboard data </param>
		public void GetLeaderboardByID(
			string leaderboardID,
			Action<List<BackEnd.Leaderboard.UserLeaderboardItem>> onGetLeaderboardData)
		{
			BackndLeaderboard.GetLeaderboardDataByID(leaderboardID, onGetLeaderboardData);
		}

		/// <summary>
		/// Get user data in specific leaderboard
		/// </summary>
		/// <param name="leaderboardID"> target leaderboard id </param>
		/// <param name="onGetMyData"> callback after success get user data </param>
		public void GetMyDataInLeaderboard(
			string leaderboardID, 
			Action<BackEnd.Leaderboard.UserLeaderboardItem> onGetMyData)
        {
			BackndLeaderboard.GetMyDataInLeaderboard(leaderboardID, onGetMyData);
		}

		/// <summary>
		/// Update user data in specific leadeboard (Synchronous)
		/// </summary>
		/// <param name="leaderboardID"> target leaderboard id </param>
		/// <param name="tableName"> table name that have field that leaderboard handle to count </param>
		/// <param name="rowIndate"> user data row in date in the table </param>
		/// <param name="column"> table field that leaderboard handle to count </param>
		/// <param name="value"> updated user data value </param>
		public void UpdateLeaderboardData(string leaderboardID, string tableName, string rowIndate, string column, long value)
			=> BackndLeaderboard.UpdateLeaderboard(leaderboardID, tableName, rowIndate, column, value);

		/// <summary>
		/// Update user data in specific leadeboard (Asynchronous)
		/// </summary>
		/// <param name="leaderboardID"> target leaderboard id </param>
		/// <param name="tableName"> table name that have field that leaderboard handle to count </param>
		/// <param name="rowIndate"> user data row in date in the table </param>
		/// <param name="column"> table field that leaderboard handle to count </param>
		/// <param name="value"> updated user data value </param>
		public void UpdateLeaderboardDataAsync(string leaderboardID, string tableName, string rowIndate, string column, long value)
			=> BackndLeaderboard.UpdateLeaderboardAsync(leaderboardID, tableName, rowIndate, column, value);
	}
}
