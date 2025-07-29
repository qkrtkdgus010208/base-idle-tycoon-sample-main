namespace Project.Gameplay.SaveData
{
	using System.Collections.Generic;
	using BackEnd;
	using UnityEngine;
	using Newtonsoft.Json;
	

	public class UserWardrobeDataManager
	{
		/// <summary>
		/// Database table name
		/// </summary>
		private const string TABLE_NAME = "UserWardrobeData";

		/// <summary>
		/// Singleton
		/// </summary>
		private static UserWardrobeDataManager _instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static UserWardrobeDataManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new UserWardrobeDataManager();
				return _instance;
			}
		}

		/// <summary>
		/// List of user wardrobe data
		/// </summary>
		private Dictionary<string, UserWardrobeData> userWardrobeDatas;

		/// <summary>
		/// List of user wardrobe data
		/// </summary>
		public Dictionary<string, UserWardrobeData> UserWardrobeDatas => userWardrobeDatas;

		/// <summary>
		/// save to local data state
		/// debug mode only
		/// </summary>
		private bool _isLocalData;

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
		/// true: if at least there is one data in userWardrobeDatas list need to save
		/// false: no data changed, no need to save
		/// </summary>
		private bool IsDataChange
		{
			get
			{
				foreach (var key in userWardrobeDatas.Keys)
					if (userWardrobeDatas[key].IsDataChange)
						return true;

				return false;
			}

			set
			{
				foreach (var key in userWardrobeDatas.Keys)
					userWardrobeDatas[key].IsDataChange = value;
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
			_ownerInDate = ownerInDate;

			userWardrobeDatas = new Dictionary<string, UserWardrobeData>();

			if (_isLocalData)
			{
				string json = PlayerPrefs.GetString(TABLE_NAME);

				if (!string.IsNullOrEmpty(json))
				{
					var datas = Utility.StaticReflection.DatabaseItemsParse<UserWardrobeData>(json);

					foreach (var dressData in datas)
						userWardrobeDatas.Add(dressData.DressID, dressData);
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

				// Data not found
				if (bro.FlattenRows().Count == 0)
					{
					// Generate new data
					Debug.Log("new UserWardrobeData");
					}
					else
					{
						var datas = Utility.StaticReflection.DatabaseItemsParse<UserWardrobeData>(bro.FlattenRows().ToJson());

						for (int i = 0; i < bro.FlattenRows().Count; i++)
						{
							if (userWardrobeDatas.ContainsKey(datas[i].DressID))
								continue;

							userWardrobeDatas.Add(datas[i].DressID, datas[i]);
						}
					}

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
						userWardrobeDatas.Clear();
				}
				else
				{
					// Load Data
					var datas = Utility.StaticReflection.DatabaseItemsParse<UserWardrobeData>(bro.FlattenRows().ToJson());

					if (!isOverride)
						userWardrobeDatas.Clear();

					for (int i = 0; i < datas.Count; i++)
					{
						if (isOverride)
						{
							if (userWardrobeDatas.ContainsKey(datas[i].DressID))
							{
								userWardrobeDatas[datas[i].DressID].inDate = datas[i].inDate;
								userWardrobeDatas[datas[i].DressID].IsDataChange = true;
							}
							else
							{
								var deltedResult = Backend.GameData.DeleteV2(TABLE_NAME, datas[i].inDate, _ownerInDate);

								if (!deltedResult.IsSuccess())
									NoticeUIController.Instance.ShowNotice(
										"Failed to delete data " + datas[i].DressID + " Message\n" + deltedResult.Message, null);
							}
						}
						else
							userWardrobeDatas.Add(datas[i].DressID, datas[i]);
					}
				}

				if (isOverride)
                {
					foreach (var key in userWardrobeDatas.Keys)
					{
						if (!userWardrobeDatas[key].IsDataChange)
						{
							userWardrobeDatas[key].inDate = string.Empty;
							userWardrobeDatas[key].IsDataChange = true;
						}
					}
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
		/// Update data in save data
		/// </summary>
		/// <param name="userData"> updated user wardrobe data </param>
		public void UpdateData(UserWardrobeData userData)
		{
			if (userWardrobeDatas.ContainsKey(userData.DressID))
				userWardrobeDatas[userData.DressID] = userData;
			else
				userWardrobeDatas.Add(userData.DressID, userData);

			userWardrobeDatas[userData.DressID].IsDataChange = true;
		}

		/// <summary>
		/// Add new data
		/// </summary>
		/// <param name="newDress"> new data to save </param>
		public void AddDressData(List<UserWardrobeData> newDress)
        {
			foreach (var dressData in newDress)
			{
				if (!userWardrobeDatas.ContainsKey(dressData.DressID))
				{
					dressData.IsDataChange = true;
					userWardrobeDatas.Add(dressData.DressID, dressData);
				}
			}

			if (IsDataChange)
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
		public void SaveData()
		{
			if (!IsDataChange || inProcessSaveData || isInteruptSaveData)
				return;

			inProcessSaveData = true;

			if (_isLocalData)
			{
				var datas = new List<UserWardrobeData>();

				foreach (var dressID in userWardrobeDatas.Keys)
				{
					datas.Add(userWardrobeDatas[dressID]);
					userWardrobeDatas[dressID].IsDataChange = false;
				}

				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(datas));
				return;
			}

			foreach (var dressID in userWardrobeDatas.Keys)
			{
				if (!userWardrobeDatas[dressID].IsDataChange)
					continue;

				var data = userWardrobeDatas[dressID];
				var param = Utility.StaticReflection.GetParam(data);


				if (string.IsNullOrEmpty(data.inDate)) // if new data
					Backend.GameData.Insert(TABLE_NAME, param,
						(bro) =>
						{
							backendCallback?.Invoke(bro);
							if (!bro.IsSuccess()) return;

							userWardrobeDatas[dressID].inDate = bro.GetInDate();
							userWardrobeDatas[dressID].IsDataChange = false;
						});
				else // if update data
					Backend.GameData.UpdateV2(
						TABLE_NAME,
						data.inDate,
						_ownerInDate,
						param,
						(bro) =>
						{
							backendCallback?.Invoke(bro);
							if (!bro.IsSuccess()) return;

							userWardrobeDatas[dressID].IsDataChange = false;
						});
			}
		}

		/// <summary>
		/// Saving data action (Synchronous)
		/// </summary>
		public void SaveDataSynchronous()
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
				var datas = new List<UserWardrobeData>();

				foreach (var dressID in userWardrobeDatas.Keys)
				{
					datas.Add(userWardrobeDatas[dressID]);
					userWardrobeDatas[dressID].IsDataChange = false;
				}

				PlayerPrefs.SetString(TABLE_NAME, JsonConvert.SerializeObject(datas));
				ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);

				return;
			}

			foreach (var dressID in userWardrobeDatas.Keys)
			{
				if (!userWardrobeDatas[dressID].IsDataChange)
					continue;

				var data = userWardrobeDatas[dressID];
				var param = Utility.StaticReflection.GetParam(data);

				if (string.IsNullOrEmpty(data.inDate)) // if new data
				{
					var bro = Backend.GameData.Insert(TABLE_NAME, param);
					
					if (!bro.IsSuccess())
					{
						NoticeUIController.Instance.ShowNotice(
							  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
							  () => SaveDataSynchronous()
						  );

						return;
					}

					userWardrobeDatas[dressID].inDate = bro.GetInDate();
					userWardrobeDatas[dressID].IsDataChange = false;

					ApplicationEvents.Instance.SetDataOnProcessState(TABLE_NAME, false);
				}
				else // if update data
				{
					var bro = Backend.GameData.UpdateV2(
								TABLE_NAME,
								data.inDate,
								_ownerInDate,
								param);

					if (!bro.IsSuccess())
					{
						NoticeUIController.Instance.ShowNotice(
							  Utility.StaticConstantDictionary.FAILED_SAVE_DATA_MESSAGE,
							  () => SaveDataSynchronous()
						  );

						return;
					}

					userWardrobeDatas[dressID].IsDataChange = false;
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

			foreach (var dressID in userWardrobeDatas.Keys)
			{
				var data = userWardrobeDatas[dressID];

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
		/// Get specific dress data by id
		/// </summary>
		/// <param name="id"> target dress id </param>
		/// <returns> dress data </returns>
		public UserWardrobeData GetDressDataByID(string id)
			=> userWardrobeDatas.ContainsKey(id) ? userWardrobeDatas[id] : null;
	}
}
