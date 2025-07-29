namespace Project.Gameplay.SaveData
{
    using BackEnd;
    using UnityEngine;
	using Newtonsoft.Json;

    public class UserDataManager
	{
		/// <summary>
		/// Database table name
		/// </summary>
		private const string TABLE_NAME = "UserMainData";

		/// <summary>
		/// Singleton
		/// </summary>
		private static UserDataManager _instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static UserDataManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new UserDataManager();
				return _instance;
			}
		}

		/// <summary>
		/// User data
		/// </summary>
		private UserData _userData;

		/// <summary>
		/// User data row in date in this save data table
		/// </summary>
		public string TableRowIndate => _userData.inDate;

		/// <summary>
		/// User current used profile image id
		/// </summary>
		public string UsedProfileImageID => _userData.UsedProfileImageID;

		/// <summary>
		/// User current used dress id
		/// </summary>
		public string UsedDressID => _userData.UsedDressID;

		/// <summary>
		/// Total coin that player have earned
		/// </summary>
		public long TotalCoinEarned => _userData.TotalCoinsEarned;

		/// <summary>
		/// account user in date
		/// </summary>
		private string _ownerInDate;

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
					_userData = JsonConvert.DeserializeObject<UserData>(json);
				else
					_userData = new UserData();

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
						// Generate new data
						Debug.Log("Generate new data UserData");

						isNewData = true;
						_userData = new UserData();
					}
					else
					{
						// Load Data

						_userData = JsonConvert.DeserializeObject<UserData>(bro.FlattenRows()[0].ToJson());
						PlayerWallet.Initialize(new Currency() { CurrencyID = Currency.ID.Gems, Amount = _userData.Gems });
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
					// Generate new data
					Debug.Log("Generate new data UserData");

					isNewData = true;

					if (!isOverride)
						_userData = new UserData();
					else
					{
						_userData.inDate = string.Empty;
						isDataChange = true;
					}
				}
				else
				{
					// Load Data
					var userData = JsonConvert.DeserializeObject<UserData>(bro.FlattenRows()[0].ToJson());
					
					isDataChange = true;
					if (isOverride)
						_userData.inDate = userData.inDate;
					else
						_userData = userData;

					PlayerWallet.Initialize(new Currency() { CurrencyID = Currency.ID.Gems, Amount = _userData.Gems });
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
			ApplicationEvents.Instance.RequestSaveData += () => SaveDataSynchronous();

			ApplicationEvents.Instance.OnPause += (isPause) => { if (isPause) SaveData(); };
			ApplicationEvents.Instance.OnExit += () => SaveData();

			ApplicationEvents.Instance.DeleteAllGameData += DeletedData;
		}

		/// <summary>
		/// Update player currency
		/// </summary>
		/// <param name="currency"> currency data </param>
		/// <param name="saveDataImmediately"> save Data Immediately state </param>
		public void UpdatePlayerCurrencyData(Currency currency, bool saveDataImmediately)
        {
            switch (currency.CurrencyID)
            {
				case Currency.ID.Coins:
					_userData.TotalCoinsEarned += currency.Amount;
					break;
				case Currency.ID.Gems:
					_userData.Gems = currency.Amount;
					break;
			}

			isDataChange = true;

			if (saveDataImmediately)
				SaveDataImmdiately();
		}

		/// <summary>
		/// Add player total coin earned
		/// </summary>
		/// <param name="value"> more that player coin earned </param>
		public void AddPlayerTotalCoin(long value)
        {
			_userData.TotalCoinsEarned += value;
			isDataChange = true;
		}

		/// <summary>
		/// Update current used profile image
		/// </summary>
		/// <param name="profileImageID"> updated used dress id </param>
		public void UpdatePlayerUsedProfileImageID(string profileImageID)
		{
			_userData.UsedProfileImageID = profileImageID;
			isDataChange = true;

			SaveDataImmdiately();
		}

		/// <summary>
		/// Update current used dress id
		/// </summary>
		/// <param name="dressID"> updated used dress id </param>
		/// <param name="saveDataImmediately"> save Data Immediately state </param>
		public void UpdateUsedDressID(string dressID, bool saveDataImmediately)
		{
			_userData.UsedDressID = dressID;
			isDataChange = true;

			if (saveDataImmediately)
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
				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(_userData));
				isDataChange = false;
				return;
			}

			if (isNewData)
				Backend.GameData.Insert(TABLE_NAME, Utility.StaticReflection.GetParam(_userData),
					(bro) =>
					{
						backendCallback?.Invoke(bro);

						if (!bro.IsSuccess())
							return;

						_userData.inDate = bro.GetInDate();
						isNewData = false;
						isDataChange = false;
					});
			else
				Backend.GameData.UpdateV2(
					TABLE_NAME,
					_userData.inDate,
					_ownerInDate,
					Utility.StaticReflection.GetParam(_userData),
					(bro) =>
					{
						backendCallback?.Invoke(bro);

						if (!bro.IsSuccess())
							return;

						isDataChange = false;
					});

			LeaderboardCoinsAccumulation.OnCoinsUpdated?.Invoke(_userData.inDate, _userData.TotalCoinsEarned);
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
				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(_userData));
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);

				return;
			}

			if (isNewData)
			{
				var bro = Backend.GameData.Insert(TABLE_NAME, Utility.StaticReflection.GetParam(_userData));

				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice(
						  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
						  () => SaveDataSynchronous()
					  );

					return;
				}

				_userData.inDate = bro.GetInDate();
				isNewData = false;
				isDataChange = false;

				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
			}
			else
			{
				var bro = Backend.GameData.UpdateV2(
							TABLE_NAME,
							_userData.inDate,
							_ownerInDate,
							Utility.StaticReflection.GetParam(_userData));

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

			LeaderboardCoinsAccumulation.OnCoinsUpdated?.Invoke(_userData.inDate, _userData.TotalCoinsEarned);
		}

		/// <summary>
		/// Delete data saved
		/// </summary>
		private void DeletedData()
		{
			ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, true);

			if (!string.IsNullOrEmpty(_userData.inDate))
			{
				var bro = Backend.GameData.DeleteV2(TABLE_NAME, _userData.inDate, _ownerInDate);
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
