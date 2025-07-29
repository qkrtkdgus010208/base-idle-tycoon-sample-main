namespace Project.StartScene
{
    using Project.BackndServer;
    using Project.Gameplay.SaveData;
    using System.Collections;
	using UnityEngine;

    public class DataLoader : MonoBehaviour
    {
        /// <summary>
        /// SO image dictionary
        /// </summary>
        [SerializeField] private Utility.StaticImageDictionary _imageDictionary;

        /// <summary>
        /// SO all of chart data
        /// </summary>
        [SerializeField] private Gameplay.SO_ChartData[] _chartDatas;

        /// <summary>
        /// SO all of probability data
        /// </summary>
        [SerializeField] private Gameplay.SO_ProbabilityData[] _probabilityDatas;

        /// <summary>
        /// SO stages default data
        /// </summary>
        [SerializeField] private Gameplay.SO_StageDefaultDatasCollections _stagesDefaultDataCollection;

        /// <summary>
        /// SO stage load object data
        /// </summary>
        [SerializeField] private SO_StageLoadData _stageDataLoad;

        /// <summary>
        /// Local data state (debug mode only)
        /// </summary>
        private static bool isLocalData;

        /// <summary>
        /// Local data state (debug mode only)
        /// </summary>
        public static bool IsLocalData
        {
            set => isLocalData = value;
            get => isLocalData;
        }

        /// <summary>
        /// is splash sreen done state
        /// </summary>
        private bool isSplashScreenDone;

        /// <summary>
        /// is player enter the game state
        /// </summary>
        private bool enterTheGame;


        private void Awake()
        {
            StaticStartSceneEventsManager.OnSplashScreenDone = () => isSplashScreenDone = true; // Assign OnSplashScreenDone event
            StaticStartSceneEventsManager.OnEnterTheGame = () => enterTheGame = true; // Assign OnEnterTheGame event
        }

        private void Start()
        {
            StartCoroutine(Initialize()); // Start initialize all data
        }

        /// <summary>
        /// Initialize all data before start the game
        /// </summary>
        /// <returns></returns>
        private IEnumerator Initialize()
        {
            yield return new WaitUntil(() => isSplashScreenDone); // await for splash screen done

            yield return CheckingGameState(); // Checking game and server state
            yield return LoginProcess(); // Process Login
            yield return LoadGameData(); // Load game data
        }

        /// <summary>
        /// Checking game & server state
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckingGameState()
        {
            BackndInitializer.Initialize(); // Initialize backnd sdk
            yield return new WaitUntil(() => BackndInitializer.IsInitialize);

            StaticEventIAP.InitializeIAPManager?.Invoke();

            BackndServerState.CheckServerStatus(StaticStartSceneEventsManager.ShowMaintenanceNotice); // Checking server state
            yield return new WaitUntil(() => BackndServerState.IsServerStatusAvailable);

            BackndServerState.CheckVersionStatus(); // Checking game update state
            yield return new WaitUntil(() => BackndServerState.IsCurrentGameVersionValid);
        }

        /// <summary>
        /// Process Login
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoginProcess()
        {
            bool manualLogin = false;
            if (BackndLogin.IsLoginWithTokenSuccess()) // if token login success, set active tap to start UI
                StaticStartSceneEventsManager.SetActiveEnterTheGameUI?.Invoke();
            else // if token login failed, then manual login required. set active login UI
            {
                StaticStartSceneEventsManager.SetActiveLoginUI?.Invoke();
                manualLogin = true;
            }

            // Await until player login
            // or if player token login success, await untill player tap enter to start
            yield return new WaitUntil(() => 
                (manualLogin || enterTheGame) && 
                (BackEnd.Backend.IsLogin || IsLocalData)
                );

            ApplicationEvents.Instance.SetPlayerEnterGameState(true); // Set player enter the game state to true
        }

        /// <summary>
        /// Load game data
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadGameData()
        {
            LoadingUIController.Instance.Loading(); // Start Loading

            if (!isLocalData)
            {
                BackndUserInfo.Instance.LoadData(); // Load account user info
                yield return new WaitUntil(() => BackndUserInfo.Instance.IsDataLoaded);

                BackndNotification.Initialize(); // Initialize backnd notification

                var chartTables = BackndChart.GetChartDatas(); // Get all chart table
                yield return new WaitUntil(() => chartTables != null);
                foreach (var chartData in _chartDatas) // Assign chart table data to each SO chart data
                    chartData.Initialize(
                        chartTables.Find(x => string.Equals(x.selectedChartFileId, chartData.ChartFileID)).contentString); // Lookup by selected chart file id

                var probabilityTables = BackndProbability.GetProbabilityContentData(); // Get all probability table
                foreach (var probabilityData in _probabilityDatas) // Assign probability table data to each SO probability data
                    probabilityData.Initialize(
                        probabilityTables.Find(x => string.Equals(x.selectedProbabilityFileId, probabilityData.FileID)).contentString); // Lookup by selected probability file id
            }

            string ownerInDate = BackndUserInfo.Instance.UserInfo.inDate; // Get user inDate

            UserDataManager.Instance.LoadData(IsLocalData, ownerInDate); // load game data (UserMainData)
            UserStageDataManager.Instance.LoadData(IsLocalData, ownerInDate); // load game data (UserStageData)
            UserDailyLoginDataManager.Instance.LoadData(IsLocalData, ownerInDate); // load game data (UserDailyLoginData)
            UserWardrobeDataManager.Instance.LoadData(IsLocalData, ownerInDate); // load game data (UserWardrobeData)

            yield return new WaitUntil(() => UserDataManager.Instance.IsDataLoaded); // await game data loaded (UserMainData)
            yield return new WaitUntil(() => UserStageDataManager.Instance.IsDataLoaded); // await game data loaded (UserStageData)
            yield return new WaitUntil(() => UserDailyLoginDataManager.Instance.IsDataLoaded); // await game data loaded (UserDailyLoginData)
            yield return new WaitUntil(() => UserWardrobeDataManager.Instance.IsDataLoaded); // await game data loaded (UserWardrobeData)

            // Look up stage to load
            string stageToLoad = _stagesDefaultDataCollection.StageDataCollection[0].StageID; // Assign to first stage (handler if player have no stage data yet)
            for (int i = _stagesDefaultDataCollection.StageDataCollection.Count - 1; i > 0; i--) // Look up for highest stage level first
            {
                if (UserStageDataManager.Instance.HasStageDataID(_stagesDefaultDataCollection.StageDataCollection[i].StageID)) // If player has saved stage id
                {
                    stageToLoad = _stagesDefaultDataCollection.StageDataCollection[i].StageID; // Set stageToLoad to highest stage level stage id found in UserStageData
                    break;
                }
            }

            _stageDataLoad.SetStageID(stageToLoad); // Set stage id to load into data loader
            _stageDataLoad.SetLastPlayerLogoutDate(UserStageDataManager.Instance.LastUpdateDataBeforeLogin); // Set last player logout date

            _imageDictionary.InitializeSingleton(); // Initialize image dictionary

            Utility.StaticConstantDictionary.MIDLE_SCREEN_POSITION = new Vector2(Screen.width / 2, Screen.height / 2); // Get device midle screen position

            StaticAudioEvents.SetAudioState?.Invoke(true); // Play bgm
            
            LoadingUIController.Instance.FinishLoading(); // End Loading

            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Utility.StaticConstantDictionary.SCENE_GAMEPLAY_IDX); // go to gameplay scene
        }
    }
}
