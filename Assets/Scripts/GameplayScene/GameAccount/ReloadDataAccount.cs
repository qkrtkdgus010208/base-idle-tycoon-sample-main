namespace Project.Gameplay
{
    using Project.BackndServer;
    using SaveData;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class ReloadDataAccount
    {
        /// <summary>
        /// Reload gameplay data process
        /// </summary>
        /// <returns></returns>
        public static IEnumerator ReloadGameplayData(
            SO_StageDefaultDatasCollections stagesDefaultDataCollection,
            SO_StageLoadData stageDataLoad,
            bool isOverride)
        {
            LoadingUIController.Instance.Loading(); // Start Loading

            ApplicationEvents.Instance.SetInteruptSaveData(true);
            PlayerWallet.ResetData();

            BackndUserInfo.Instance.LoadData(); // Load account user info
            yield return new WaitUntil(() => BackndUserInfo.Instance.IsDataLoaded);

            BackndNotification.Initialize(); // Initialize backnd notification


            string ownerInDate = BackndUserInfo.Instance.UserInfo.inDate; // Get user inDate

            UserDataManager.Instance.ReloadAccountData(isOverride, ownerInDate); // load game data (UserMainData)
            UserStageDataManager.Instance.ReloadAccountData(isOverride, ownerInDate); // load game data (UserStageData)
            UserDailyLoginDataManager.Instance.ReloadAccountData(isOverride, ownerInDate); // load game data (UserDailyLoginData)
            UserWardrobeDataManager.Instance.ReloadAccountData(isOverride, ownerInDate); // load game data (UserWardrobeData)

            yield return new WaitUntil(() => UserDataManager.Instance.IsDataLoaded); // await game data loaded (UserMainData)
            yield return new WaitUntil(() => UserStageDataManager.Instance.IsDataLoaded); // await game data loaded (UserStageData)
            yield return new WaitUntil(() => UserDailyLoginDataManager.Instance.IsDataLoaded); // await game data loaded (UserDailyLoginData)
            yield return new WaitUntil(() => UserWardrobeDataManager.Instance.IsDataLoaded); // await game data loaded (UserWardrobeData)

            // Look up stage to load
            string stageToLoad = stagesDefaultDataCollection.StageDataCollection[0].StageID; // Assign to first stage (handler if player have no stage data yet)
            for (int i = stagesDefaultDataCollection.StageDataCollection.Count - 1; i > 0; i--) // Look up for highest stage level first
            {
                if (UserStageDataManager.Instance.HasStageDataID(stagesDefaultDataCollection.StageDataCollection[i].StageID)) // If player has saved stage id
                {
                    stageToLoad = stagesDefaultDataCollection.StageDataCollection[i].StageID; // Set stageToLoad to highest stage level stage id found in UserStageData
                    break;
                }
            }

            stageDataLoad.SetStageID(stageToLoad); // Set stage id to load into data loader
            stageDataLoad.SetLastPlayerLogoutDate(UserStageDataManager.Instance.LastUpdateDataBeforeLogin); // Set last player logout date

            ApplicationEvents.Instance.SetInteruptSaveData(false);
            ApplicationEvents.Instance.SaveAllProgression();

            StaticAudioEvents.ReloadAudioVolume?.Invoke();
            StaticAudioEvents.SetAudioState?.Invoke(true);

            LoadingUIController.Instance.FinishLoading(); // Finish Loading

            yield return SceneManager.LoadSceneAsync(Utility.StaticConstantDictionary.SCENE_GAMEPLAY_IDX);
        }
    }
}
