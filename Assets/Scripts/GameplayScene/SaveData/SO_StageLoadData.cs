namespace Project.Gameplay.SaveData
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Stage Data/Stage Load Data")]
	public class SO_StageLoadData : ScriptableObject
	{
		/// <summary>
		/// Stage upgrade data collection
		/// </summary>
		[SerializeField] private List<SO_BatchStageUpgradeData> _stageUpgradeDatas;

		/// <summary>
		/// Get data current stage loaded upgrade data
		/// </summary>
		public SO_BatchStageUpgradeData CurrentStageUpgradeData
			=> _stageUpgradeDatas.Find(x => string.Equals(x.StageID, stageIdToLoad));

		/// <summary>
		/// Stage environment data collection
		/// </summary>
		[SerializeField] private List<SO_StageEnvironmentsData> _stageEnvironmentsDatas;

		/// <summary>
		/// Get current stage loaded environtment
		/// </summary>
		public SO_StageEnvironmentsData CurrentStageEnvirontmentsData 
			=> _stageEnvironmentsDatas.Find(x => string.Equals(x.StageID, stageIdToLoad));

		/// <summary>
		/// Stage kitchen data collection
		/// </summary>
		[SerializeField] private List<SO_StageKitchensData> _stageKitchenDatas;

		/// <summary>
		/// Get current stage loaded kitchen data
		/// </summary>
		public SO_StageKitchensData CurrentStageKitchensData
			=> _stageKitchenDatas.Find(x => string.Equals(x.StageID, stageIdToLoad));

		/// <summary>
		/// Stage order table data collection
		/// </summary>
		[SerializeField] private List<SO_StageOrderTablesData> _stageOrderTablesData;

		/// <summary>
		/// Get current stage loaded order table data
		/// </summary>
		public SO_StageOrderTablesData CurrentStageOrderTablesData
			=> _stageOrderTablesData.Find(x => string.Equals(x.StageID, stageIdToLoad));

		/// <summary>
		/// Stage staff data collection
		/// </summary>
		[SerializeField] private List<SO_StageStaffData> _stageStaffData;

		/// <summary>
		/// Get current stage loaded order staff data
		/// </summary>
		public SO_StageStaffData CurrentStageStaffData
			=> _stageStaffData.Find(x => string.Equals(x.StageID, stageIdToLoad));

		/// <summary>
		/// Stage customer data collection
		/// </summary>
		[SerializeField] private List<SO_StageCustomerData> _stageCustomerData;

		/// <summary>
		/// Get current stage loaded order customer data
		/// </summary>
		public SO_StageCustomerData CurrentStageCustomerData
			=> _stageCustomerData.Find(x => string.Equals(x.StageID, stageIdToLoad));

		/// <summary>
		/// stage ID to load
		/// </summary>
		private string stageIdToLoad;

		/// <summary>
		/// Get current loaded stage ID
		/// </summary>
		public string CurrentStageID => stageIdToLoad;

		/// <summary>
		/// Get last player logout date
		/// </summary>
		private DateTime lastPlayerLogoutDate;

		/// <summary>
		/// Get last player logout date
		/// </summary>
		public DateTime LastPlayerLogoutDate => lastPlayerLogoutDate;

		/// <summary>
		/// Set stage id to load
		/// </summary>
		/// <param name="stageID"> stage id to load </param>
		public void SetStageID(string stageID) 
			=> stageIdToLoad = stageID;

		/// <summary>
		/// Set last player logout date
		/// </summary>
		/// <param name="lastLogout"> last player logout date </param>
		public void SetLastPlayerLogoutDate(DateTime lastLogout) 
			=> lastPlayerLogoutDate = lastLogout;
	}
}
