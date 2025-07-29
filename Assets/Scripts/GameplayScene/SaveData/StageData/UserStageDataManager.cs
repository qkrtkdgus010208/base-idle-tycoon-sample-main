namespace Project.Gameplay.SaveData
{
	using System.Collections.Generic;
	using BackEnd;
    using UnityEngine;
	using Newtonsoft.Json;
    using System;

    public class UserStageDataManager
	{
		/// <summary>
		/// Database table name
		/// </summary>
		private const string TABLE_NAME = "UserStageData";

		/// <summary>
		/// Singleton
		/// </summary>
		private static UserStageDataManager _instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static UserStageDataManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new UserStageDataManager();
				return _instance;
			}
		}

		/// <summary>
		/// list of user stage data
		/// </summary>
		private Dictionary<string, UserStageObjectData> userStageObjectDatas;

		/// <summary>
		/// account user in date
		/// </summary>
		private string _ownerInDate;

		/// <summary>
		/// data failed to send counter
		/// </summary>
		private int sendDataFailedCount;

		/// <summary>
		/// Is data change and need to save
		/// true: if at least there is one data in userStageObjectDatas list need to save
		/// false: no data changed, no need to save
		/// </summary>
		private bool IsDataChange
		{
			get
			{
				foreach (var key in userStageObjectDatas.Keys)
					if (userStageObjectDatas[key].IsDataChange)
						return true;

				return false;
			}

            set
            {
				foreach (var key in userStageObjectDatas.Keys)
					userStageObjectDatas[key].IsDataChange = false;
			}
		}

		/// <summary>
		/// Interupt save state
		/// true: save data not allowed
		/// false: save data allowed
		/// </summary>
		private bool isInteruptSaveData;

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
		/// Last data updated before this login session
		/// </summary>
		private DateTime lastUpdateDataBeforeLogin;

		/// <summary>
		/// Last data updated before this login session
		/// </summary>
		public DateTime LastUpdateDataBeforeLogin => lastUpdateDataBeforeLogin;

		/// <summary>
		/// Load data
		/// </summary>
		/// <param name="isLocalData"> is local data state (debug mode only) </param>
		/// <param name="ownerInDate"> user in date </param>
		public void LoadData(bool isLocalData, string ownerInDate)
		{
			_isDataLoaded = false;
			_isLocalData = isLocalData;
			userStageObjectDatas = new Dictionary<string, UserStageObjectData>();

			if (_isLocalData)
			{
				string json = PlayerPrefs.GetString(TABLE_NAME);

				if (!string.IsNullOrEmpty(json))
				{
					var datas = Utility.StaticReflection.DatabaseItemsParse<UserStageData>(json);

					foreach (var stageData in datas)
					{
						var stageObject = new UserStageObjectData(stageData);

						PlayerWallet.Initialize(new Currency()
						{
							CurrencyID = Currency.ID.Coins,
							StageID = stageObject.StageID,
							Amount = stageObject.PlayerCoin
						});

						userStageObjectDatas.Add(stageObject.StageID, stageObject);
					}
				}

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

					if (bro.FlattenRows().Count == 0)
					{
						// Generate new data
						Debug.Log("new UserStageData");
					}
					else
					{
						var datas = bro.FlattenRows();
						lastUpdateDataBeforeLogin = DateTime.Parse(datas[0][Utility.StaticConstantDictionary.LAST_UPDATED_DATE_TIME_KEY].ToString());

						// Load Data
						for (int i = 0; i < datas.Count; i++)
						{
							var stageData = JsonConvert.DeserializeObject<UserStageData>(datas[i].ToJson());
							
							var lastUpdate = DateTime.Parse(datas[i][Utility.StaticConstantDictionary.LAST_UPDATED_DATE_TIME_KEY].ToString());
							if (lastUpdate.Ticks > lastUpdateDataBeforeLogin.Ticks)
								lastUpdateDataBeforeLogin = lastUpdate;

							var stageObject = new UserStageObjectData(stageData);

							PlayerWallet.Initialize(new Currency()
							{
								CurrencyID = Currency.ID.Coins,
								StageID = stageObject.StageID,
								Amount = stageObject.PlayerCoin
							});

							userStageObjectDatas.Add(stageObject.StageID, stageObject);
						}
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
					if (!isOverride)
						userStageObjectDatas.Clear();
				}
				else
				{
					// Load Data
					var datas = bro.FlattenRows();

					if (!isOverride)
					{
						lastUpdateDataBeforeLogin = DateTime.Parse(datas[0][Utility.StaticConstantDictionary.LAST_UPDATED_DATE_TIME_KEY].ToString());
						userStageObjectDatas.Clear();
					}
					else
						lastUpdateDataBeforeLogin = BackndServer.BackndServerTime.GetServerTime();

					for (int i = 0; i < datas.Count; i++)
					{
						var stageData = JsonConvert.DeserializeObject<UserStageData>(datas[i].ToJson());

						if (isOverride)
						{
							if (userStageObjectDatas.ContainsKey(stageData.StageID))
							{
								userStageObjectDatas[stageData.StageID].InDate = stageData.inDate;

								PlayerWallet.Initialize(new Currency()
								{
									CurrencyID = Currency.ID.Coins,
									StageID = stageData.StageID,
									Amount = stageData.PlayerCoin
								});

								userStageObjectDatas[stageData.StageID].IsDataChange = true;
							}
							else
							{
								var deltedResult = Backend.GameData.DeleteV2(TABLE_NAME, stageData.inDate, _ownerInDate);

								if (!deltedResult.IsSuccess())
									NoticeUIController.Instance.ShowNotice(
										"Failed to delete data " + stageData.StageID + " Message\n" + deltedResult.Message, null);
							}
						}
						else
						{
							var lastUpdate = DateTime.Parse(datas[i][Utility.StaticConstantDictionary.LAST_UPDATED_DATE_TIME_KEY].ToString());

							if (lastUpdate.Ticks > lastUpdateDataBeforeLogin.Ticks)
								lastUpdateDataBeforeLogin = lastUpdate;

							var stageObject = new UserStageObjectData(stageData);
							userStageObjectDatas.Add(stageData.StageID, stageObject);

							PlayerWallet.Initialize(new Currency()
							{
								CurrencyID = Currency.ID.Coins,
								StageID = stageData.StageID,
								Amount = stageData.PlayerCoin
							});
						}
					}
				}

				if (isOverride)
				{
					foreach (var key in userStageObjectDatas.Keys)
					{
						if (!userStageObjectDatas[key].IsDataChange)
						{
							userStageObjectDatas[key].InDate = string.Empty;
							userStageObjectDatas[key].IsDataChange = true;
						}
					}
				}

				
				_isDataLoaded = true;
			});
		}

		/// <summary>
		/// Get temporary login account progression
		/// </summary>
		/// <returns> list of temporary login account stage data </returns>
		public List<UserStageData> GetPlayerUserStageTemporaryLoginData()
		{
			var userStageData = new List<UserStageData>();
			var bro = Backend.GameData.GetMyData(TABLE_NAME, new Where());

			if (!bro.IsSuccess())
				Debug.Log(bro.Message);
			else if (bro.FlattenRows().Count > 0)
			{
				var datas = bro.FlattenRows();
				for (int i = 0; i < datas.Count; i++)
					userStageData.Add(JsonConvert.DeserializeObject<UserStageData>(datas[i].ToJson()));
			}

			return userStageData;
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
		/// Add new stage data
		/// </summary>
		/// <param name="stageID"> stage id that will added </param>
		/// <param name="playerCoin"> player starting coin in that stage </param>
		public void AddStageData(string stageID, long playerCoin = 0)
		{
			var stageData = new UserStageData() { StageID = stageID, PlayerCoin = playerCoin, KitchenDatas = "[]", UpgradeLevel = "[]", };
			var stageObject = new UserStageObjectData(stageData);
			stageObject.IsDataChange = true;

			userStageObjectDatas.Add(stageID, stageObject);
		}

		/// <summary>
		/// Update kitchen data in specific stage
		/// </summary>
		/// <param name="stageID"> target stage id </param>
		/// <param name="kitchenID"> target kitchen id that need to update </param>
		/// <param name="kitchenLevel"> updated kitchen level </param>
		public void UpdateData(string stageID, string kitchenID, int kitchenLevel)
		{
			if (!userStageObjectDatas.ContainsKey(stageID))
				AddStageData(stageID);

			if (userStageObjectDatas[stageID].KitchenDatas == null)
				userStageObjectDatas[stageID].KitchenDatas = new List<UserStageObjectData.KitchenDataInStage>();

			var kitchenData = userStageObjectDatas[stageID].KitchenDatas.Find(x => string.Equals(x.KitchenDataID, kitchenID));
			if (kitchenData == null)
				userStageObjectDatas[stageID].KitchenDatas.Add(
					new UserStageObjectData.KitchenDataInStage() { 
						KitchenDataID = kitchenID, 
						KitchenDataLevel = kitchenLevel
					});
			else
				kitchenData.KitchenDataLevel = kitchenLevel;

			userStageObjectDatas[stageID].IsDataChange = true;
		}

		/// <summary>
		/// Update specific stage upgrade data
		/// </summary>
		/// <param name="stageID"> target stage id </param>
		/// <param name="upgradeLevel"> upgrade level that unlocked </param>
		public void UpdateData(string stageID, int upgradeLevel)
		{
			if (!userStageObjectDatas.ContainsKey(stageID))
				AddStageData(stageID);

			if (userStageObjectDatas[stageID].UpgradeLevel == null)
				userStageObjectDatas[stageID].UpgradeLevel = new List<int>();

			if (!userStageObjectDatas[stageID].UpgradeLevel.Contains(upgradeLevel))
				userStageObjectDatas[stageID].UpgradeLevel.Add(upgradeLevel);


			userStageObjectDatas[stageID].IsDataChange = true;
		}

		/// <summary>
		/// Update player coin in specific stage
		/// </summary>
		/// <param name="coin"> currency data </param>
		/// <param name="saveDataImmediately"> save Data Immediately state </param>
		public void UpdatePlayerCoinInStage(Currency coin, bool saveDataImmediately)
		{
			if (!userStageObjectDatas.ContainsKey(coin.StageID))
				AddStageData(coin.StageID);

			userStageObjectDatas[coin.StageID].PlayerCoin = coin.Amount;
			userStageObjectDatas[coin.StageID].IsDataChange = true;

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
			if (!IsDataChange || inProcessSaveData || isInteruptSaveData)
				return;

			inProcessSaveData = true;

			if (_isLocalData)
			{
				var datas = new List<UserStageData>();

				foreach (var stageID in userStageObjectDatas.Keys)
				{
					datas.Add(userStageObjectDatas[stageID].GetUserStageData());
					userStageObjectDatas[stageID].IsDataChange = false;
				}

				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(datas));
				
				inProcessSaveData = false;
				return;
			}

			foreach (var stageID in userStageObjectDatas.Keys)
			{
				if (!userStageObjectDatas[stageID].IsDataChange)
					continue;

				var data = userStageObjectDatas[stageID].GetUserStageData();
				var param = Utility.StaticReflection.GetParam(data);

				if (string.IsNullOrEmpty(data.inDate)) // if new data
					Backend.GameData.Insert(TABLE_NAME, Utility.StaticReflection.GetParam(data),
						(bro) =>
						{
							backendCallback?.Invoke(bro);
							if (!bro.IsSuccess()) return;

							userStageObjectDatas[stageID].InDate = bro.GetInDate();
							userStageObjectDatas[stageID].IsDataChange = false;
						});
				else // if update data
					Backend.GameData.UpdateV2(
						TABLE_NAME,
						data.inDate,
						_ownerInDate,
						Utility.StaticReflection.GetParam(data),
						(bro) =>
						{
							backendCallback?.Invoke(bro);
							if (!bro.IsSuccess()) return;

							userStageObjectDatas[stageID].IsDataChange = false;
						});
			}
		}

		/// <summary>
		/// Saving data action (Synchronous)
		/// </summary>
		private void SaveDataSynchronous()
		{
			if (isInteruptSaveData)
				return;

			ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, true);

			if (!IsDataChange)
			{
				Debug.Log("Progression Saved : " + TABLE_NAME);
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);

				return;
			}

			if (_isLocalData)
			{
				var datas = new List<UserStageData>();

				foreach (var stageID in userStageObjectDatas.Keys)
				{
					datas.Add(userStageObjectDatas[stageID].GetUserStageData());
					userStageObjectDatas[stageID].IsDataChange = false;
				}

				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(datas));
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
				return;
			}

			foreach (var stageID in userStageObjectDatas.Keys)
			{
				if (!userStageObjectDatas[stageID].IsDataChange)
					continue;

				var data = userStageObjectDatas[stageID].GetUserStageData();
				var param = Utility.StaticReflection.GetParam(data);

				if (string.IsNullOrEmpty(data.inDate)) // if new data
				{
					var bro = Backend.GameData.Insert(TABLE_NAME, Utility.StaticReflection.GetParam(data));
					
					if (!bro.IsSuccess())
					{
						NoticeUIController.Instance.ShowNotice(
							  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
							  () => SaveDataSynchronous()
						  );

						return;
					}

					userStageObjectDatas[stageID].InDate = bro.GetInDate();
					userStageObjectDatas[stageID].IsDataChange = false;

					ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
				}
				else // if update data
				{
					var bro = Backend.GameData.UpdateV2(
								TABLE_NAME,
								data.inDate,
								_ownerInDate,
								Utility.StaticReflection.GetParam(data));

					if (!bro.IsSuccess())
					{
						NoticeUIController.Instance.ShowNotice(
							  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
							  () => SaveDataSynchronous()
						  );

						return;
					}

					userStageObjectDatas[stageID].IsDataChange = false;
					ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
				}
			}
		}

		/// <summary>
		/// Delete all data saved
		/// </summary>
		private void DeletedData()
		{
			ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, true);

			foreach (var stageID in userStageObjectDatas.Keys)
			{
				var data = userStageObjectDatas[stageID].GetUserStageData();

				if (!string.IsNullOrEmpty(data.inDate))
				{
					var bro = Backend.GameData.DeleteV2(TABLE_NAME, data.inDate, _ownerInDate);
					Debug.Log("Deleted Data " + TABLE_NAME + " State: " + bro.IsSuccess());
				}
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

		/// <summary>
		/// Checking if player already unlock specific stage
		/// </summary>
		/// <param name="stageID"> target stage id </param>
		/// <returns> true: unlocked / false: locked </returns>
        public bool HasStageDataID(string stageID) 
			=> userStageObjectDatas.ContainsKey(stageID);

		/// <summary>
		/// Get specific stage data
		/// </summary>
		/// <param name="stageID"> target stage id </param>
		/// <returns> stage data </returns>
		public UserStageObjectData GetStageDataByID(string stageID)
        {
			if (userStageObjectDatas.ContainsKey(stageID))
				return userStageObjectDatas[stageID];

			return null;
        }

		/// <summary>
		/// Stage object data
		/// Object after transform user stage data from database
		/// </summary>
		public class UserStageObjectData
		{
			/// <summary>
			/// backnd's data id
			/// </summary>
			public string InDate;

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
			public List<int> UpgradeLevel;

			/// <summary>
			/// kitchen station datas
			/// </summary>
			public List<KitchenDataInStage> KitchenDatas;

			/// <summary>
			/// Data update state
			/// </summary>
			public bool IsDataChange;

			public UserStageObjectData(UserStageData userStageData)
            {
				InDate = userStageData.inDate;
				StageID = userStageData.StageID;
				PlayerCoin = userStageData.PlayerCoin;
				
				UpgradeLevel = new List<int>();
				var levelDataList = Utility.StaticReflection.DatabaseItemsParse<int>(userStageData.UpgradeLevel);
				foreach (var level in levelDataList)
					UpgradeLevel.Add(level);

				KitchenDatas = new List<KitchenDataInStage>();
				var kitchenDataList = Utility.StaticReflection.DatabaseItemsParse<UserStageObjectData.KitchenDataInStage>(userStageData.KitchenDatas);
				foreach (var kitchen in kitchenDataList)
					KitchenDatas.Add(kitchen);
			}

			/// <summary>
			/// Get this stage data
			/// </summary>
			/// <returns> stage data ready to save into database </returns>
			public UserStageData GetUserStageData()
				=> new UserStageData()
				{
					inDate = InDate,
					StageID = StageID,
					PlayerCoin = PlayerCoin,
					UpgradeLevel = JsonConvert.SerializeObject(UpgradeLevel),
					KitchenDatas = JsonConvert.SerializeObject(KitchenDatas)
				};
			

			[Serializable]
			public class KitchenDataInStage
			{
				/// <summary>
				/// kitchen primary key
				/// </summary>
				public string KitchenDataID;

				/// <summary>
				/// kitchen level
				/// </summary>
				public int KitchenDataLevel;
			}
		}
	}
}
