namespace Project.Gameplay.SaveData
{
    using BackEnd;
    using UnityEngine;
	using Newtonsoft.Json;

    public class UserDailyLoginDataManager
	{
		/// <summary>
		/// Database table name
		/// </summary>
		private const string TABLE_NAME = "UserDailyLoginData";

		/// <summary>
		/// Singleton
		/// </summary>
		private static UserDailyLoginDataManager _instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static UserDailyLoginDataManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new UserDailyLoginDataManager();
				return _instance;
			}
		}

		/// <summary>
		/// User login data
		/// </summary>
		private UserDailyLoginData _userLoginData;

		/// <summary>
		/// account user in date
		/// </summary>
		private string _ownerInDate;

		/// <summary>
		/// user daily reward idx
		/// </summary>
		public int CurrentDailyRewardIdx => _userLoginData.DailyRewardIdx;
		
		/// <summary>
		/// last user claimed daily reward date
		/// </summary>
		public string LastClaimedDailyRewardDate => _userLoginData.LastClaimedDailyRewardDate;

		/// <summary>
		/// data failed to send counter
		/// </summary>
		private int sendDataFailedCount;

		/// <summary>
		/// Interupt save state
		/// true: save data not allowed
		/// false: save data allowed
		/// </summary>
		private bool isInteruptSaveData;

		/// <summary>
		/// Is data change and need to save
		/// true: need to save
		/// false: no need to save
		/// </summary>
		private bool isDataChange;

		/// <summary>
		/// Save data in process state
		/// true: save data in process, need to wait untill finished to next save
		/// false: data can be save right now
		/// </summary>
		private bool inProcessSaveData;

		/// <summary>
		/// Immediately saving state
		/// true when immediate save in process
		/// </summary>
		private bool isImmediatelySaving;

		/// <summary>
		/// save to local data state
		/// debug mode only
		/// </summary>
		private bool _isLocalData;

		/// <summary>
		/// is new data state
		/// </summary>
		private bool isNewData;

		/// <summary>
		/// Data load state
		/// true: indicate data ready to get
		/// </summary>
		private bool _isDataLoaded;

		/// <summary>
		/// Data load state
		/// true: indicate data ready to get
		/// </summary>
		public bool IsDataLoaded => _isDataLoaded;

		/// <summary>
		/// Load data
		/// </summary>
		/// <param name="isLocalData"> is local data state (debug mode only) </param>
		/// <param name="ownerInDate"> user in date </param>
		public void LoadData(bool isLocalData, string ownerInDate)
		{
			_isDataLoaded = false;
			_isLocalData = isLocalData;

			if (_isLocalData)
			{
				string json = PlayerPrefs.GetString(TABLE_NAME);

				if (!string.IsNullOrEmpty(json))
					_userLoginData = JsonConvert.DeserializeObject<UserDailyLoginData>(json);
				else
					_userLoginData = new UserDailyLoginData();

				SetupCompleted();
			}
			else
			{
				Backend.GameData.GetMyData(TABLE_NAME, new Where(), (bro) =>
				{
					if (!bro.IsSuccess())
					{
						Debug.Log(bro.StatusCode);
						return;
					}

					// Data not found
					if (bro.FlattenRows().Count == 0)
					{
						isNewData = true;
						_userLoginData = new UserDailyLoginData();
					}
					else
					{
						// Load Data
						_userLoginData = JsonConvert.DeserializeObject<UserDailyLoginData>(bro.FlattenRows()[0].ToJson());
					}

					_ownerInDate = ownerInDate;
					SetupCompleted();
				});
			}
		}

		/// <summary>
		/// Reload data
		/// </summary>
		/// <param name="isOverride"> override current progression to current login account </param>
		/// <param name="ownerInDate"> user in date </param>
		public void ReloadAccountData(bool isOverride, string ownerInDate)
		{
			_isDataLoaded = false;
			_ownerInDate = ownerInDate;

			Backend.GameData.GetMyData(TABLE_NAME, new Where(), (bro) =>
			{
				if (!bro.IsSuccess())
				{
					Debug.Log(bro.StatusCode);
					return;
				}

				// Data not found
				if (bro.FlattenRows().Count == 0)
				{
					isNewData = true;

					if (!isOverride)
						_userLoginData = new UserDailyLoginData();
					else
					{
						_userLoginData.inDate = string.Empty;
						isDataChange = true;
					}
				}
				else
				{
					// Load Data
					var userLoginData = JsonConvert.DeserializeObject<UserDailyLoginData>(bro.FlattenRows()[0].ToJson());
					
					isDataChange = true;
					if (isOverride)
						_userLoginData.inDate = userLoginData.inDate;
					else
						_userLoginData = userLoginData;
				}

				
				_isDataLoaded = true;
			});
		}

		/// <summary>
		/// After initialize completed
		/// </summary>
		private void SetupCompleted()
        {
			_isDataLoaded = true;

			AutoSaveTimer.OnSaveTime += () => SaveData();

			ApplicationEvents.Instance.RegisterSaveData(TABLE_NAME);

			ApplicationEvents.Instance.OnSaveDataInteruption += (state) => isInteruptSaveData = state;
			ApplicationEvents.Instance.RequestSaveData += SaveDataSynchronous;

			ApplicationEvents.Instance.OnPause += (isPause) => { if (isPause) SaveData(); };
			ApplicationEvents.Instance.OnExit += () => SaveData();

			ApplicationEvents.Instance.DeleteAllGameData += DeletedData;
		}

		/// <summary>
		/// Update user login day idx
		/// </summary>
		/// <param name="dayIdx"> updated day idx </param>
		public void UpdateLoginDayIdx(int dayIdx)
		{
			_userLoginData.DailyRewardIdx = dayIdx;
			_userLoginData.LastClaimedDailyRewardDate = BackndServer.BackndServerTime.GetServerTime().ToString();

			isDataChange = true;
			SaveDataImmdiately();
		}

		/// <summary>
		/// Save data immediately
		/// </summary>
		public void SaveDataImmdiately()
		{
			if (!isImmediatelySaving)
			{
				isImmediatelySaving = true;
				AutoSaveTimer.Instance.SaveDataImmediately(() => SaveData());
			}
		}

		/// <summary>
		/// Saving data action (Asynchronous)
		/// </summary>
		private void SaveData()
        {
			if (!isDataChange || inProcessSaveData || isInteruptSaveData)
				return;

			inProcessSaveData = true;

			if (_isLocalData)
			{
				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(_userLoginData));
				isDataChange = false;
				return;
			}

			if (isNewData)
				Backend.GameData.Insert(TABLE_NAME, Utility.StaticReflection.GetParam(_userLoginData), 
					(bro) =>
					{
						backendCallback?.Invoke(bro);

						if (!bro.IsSuccess())
							return;

						_userLoginData.inDate = bro.GetInDate();
						isNewData = false;
						isDataChange = false;
					});
			else
				Backend.GameData.UpdateV2(
					TABLE_NAME, 
					_userLoginData.inDate, 
					_ownerInDate, 
					Utility.StaticReflection.GetParam(_userLoginData),
					(bro) =>
					{
						backendCallback?.Invoke(bro);

						if (!bro.IsSuccess())
							return;

						isDataChange = false;
					});
		}

		/// <summary>
		/// Saving data action (Synchronous)
		/// </summary>
		private void SaveDataSynchronous()
        {
			if (isInteruptSaveData)
				return;

			ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, true);

			if (!isDataChange)
			{
				Debug.Log("Progression Saved : " + TABLE_NAME);
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);

				return;
			}

			if (_isLocalData)
			{
				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(_userLoginData));
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
				
				isDataChange = false;
				return;
			}

			if (isNewData)
			{
				var bro = Backend.GameData.Insert(TABLE_NAME, Utility.StaticReflection.GetParam(_userLoginData));

				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice(
						  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
						  () => SaveDataSynchronous()
					  );

					return;
				}

				_userLoginData.inDate = bro.GetInDate();
				isNewData = false;
				isDataChange = false;

				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
			}
			else
			{
				var bro = Backend.GameData.UpdateV2(
							TABLE_NAME,
							_userLoginData.inDate,
							_ownerInDate,
							Utility.StaticReflection.GetParam(_userLoginData));
				
				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice(
						  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
						  () => SaveDataSynchronous()
					  );

					return;
				}

				isDataChange = false;
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
			}
		}

		/// <summary>
		/// Delete data saved
		/// </summary>
		private void DeletedData()
		{
			ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, true);

			if (!string.IsNullOrEmpty(_userLoginData.inDate))
			{
				var bro = Backend.GameData.DeleteV2(TABLE_NAME, _userLoginData.inDate, _ownerInDate);
				Debug.Log("Deleted Data " + TABLE_NAME + " State: " + bro.IsSuccess());
			}

			ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
		}

		/// <summary>
		/// Backnd callback
		/// to handle if data failed to send
		/// </summary>
		private Backend.BackendCallback backendCallback = (bro) =>
		{
			if (bro.IsSuccess())
				Instance.sendDataFailedCount = 0;
			else
				Instance.SendDataFailed();

			Instance.inProcessSaveData = false;
		};

		/// <summary>
		/// Send data failed action
		/// </summary>
		private void SendDataFailed()
		{
			sendDataFailedCount++;

			if (sendDataFailedCount > Utility.StaticConstantDictionary.MAX_FAIL_SEND_DATA_COUNT)
			{
				NoticeUIController.Instance.ShowNotice(
					Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
					() => SaveData()
					);
			}
		}
	}
}
