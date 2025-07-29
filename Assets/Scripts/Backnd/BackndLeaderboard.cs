namespace Project.BackndServer
{
    using System;
    using System.Collections.Generic;
    using BackEnd;
    using BackEnd.Leaderboard;
    using UnityEngine;

    
    public static class BackndLeaderboard
    {
        /// <summary>
        /// Load all of leaderboard from backnd
        /// </summary>
        /// <param name="onTableLoaded"></param>
        public static void LoadLeaderBoard(Action<List<LeaderboardTableItem>> onTableLoaded)
        {
            Backend.Leaderboard.User.GetLeaderboards(bro =>
            {
                if (!bro.IsSuccess())
                {
                    Debug.LogError("Error load leaderboard :\n" + bro.Message);
                    return;
                }

                onTableLoaded?.Invoke(bro.GetLeaderboardTableList());
            });
        }

        /// <summary>
        /// Get data on specific leaderboard
        /// </summary>
        /// <param name="leaderboardID"> target leaderboard id </param>
        /// <param name="onGetLeaderboard"> callback after successful get the leaderboard data </param>
        public static void GetLeaderboardDataByID(
            string leaderboardID, 
            Action<List<UserLeaderboardItem>> onGetLeaderboard)
        {
            // Look up the 1st - 10th rank holders in the leaderboardUuid ranking
            Backend.Leaderboard.User.GetLeaderboard(leaderboardID, bro =>
            {
                if (!bro.IsSuccess())
                {
                    Debug.LogError("Error load leaderboard table " + leaderboardID + ":\n" + bro.Message);
                    return;
                }

                onGetLeaderboard?.Invoke(bro.GetUserLeaderboardList());
            });
        }

        /// <summary>
        /// Get user data on specific leaderboard
        /// </summary>
        /// <param name="leaderboardUUID"> target leaderboard id </param>
        /// <param name="onGetMyData"> callback after successful get the user data </param>
        public static void GetMyDataInLeaderboard(string leaderboardUUID, Action<UserLeaderboardItem> onGetMyData)
        {
            Backend.Leaderboard.User.GetMyLeaderboard(leaderboardUUID, bro => {
                if (!bro.IsSuccess())
                {
                    Debug.LogError("Error load my leaderboard data :" + bro.Message);
                    return;
                }

                onGetMyData?.Invoke(bro.GetUserLeaderboardList()[0]);
            });
        }

        /// <summary>
        /// Update user data on specific leaderboard (Synchronous execution)
        /// </summary>
        /// <param name="leaderboardUUID"> target leaderboard id </param>
        /// <param name="tableName"> table name that leaderboard handle to compare </param>
        /// <param name="rowIndate"> user data row in date in the table </param>
        /// <param name="column"> specific field to update & compare </param>
        /// <param name="value"> update value </param>
        public static void UpdateLeaderboard(string leaderboardUUID, string tableName, string rowIndate, string column, long value)
        {
            Param param = new Param();
            param.Add(column, value);

            if (rowIndate == string.Empty)
            {
                Debug.LogError("Leaderboard registration failed, Missing Row In Date Table");
                return;
            }

            var bro = Backend.Leaderboard.User.UpdateMyDataAndRefreshLeaderboard(leaderboardUUID, tableName, rowIndate, param);
            if (!bro.IsSuccess())
            {
                Debug.LogError("Leaderboard registration failed :\n" + bro.Message);
                return;
            }
        }

        /// <summary>
        /// Update user data on specific leaderboard (Asynchronous execution)
        /// </summary>
        /// <param name="leaderboardUUID"> target leaderboard id </param>
        /// <param name="tableName"> table name that leaderboard handle to compare </param>
        /// <param name="rowIndate"> user data row in date in the table </param>
        /// <param name="column"> specific field to update & compare </param>
        /// <param name="value"> update value </param>
        public static void UpdateLeaderboardAsync(string leaderboardUUID, string tableName, string rowIndate, string column, long value)
        {
            Param param = new Param();
            param.Add(column, value);

            if (rowIndate == string.Empty)
            {
                Debug.LogError("Leaderboard registration failed, Missing Row In Date Table");
                return;
            }

            Backend.Leaderboard.User.UpdateMyDataAndRefreshLeaderboard(leaderboardUUID, tableName, rowIndate, param,
                (bro) =>
                {
                    if (!bro.IsSuccess())
                    {
                        Debug.LogError("Leaderboard registration failed :\n" + bro.Message);
                        return;
                    }
                });
        }
    }
}
