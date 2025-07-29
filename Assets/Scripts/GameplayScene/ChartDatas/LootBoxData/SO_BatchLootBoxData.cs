namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Loot Box/Loot Box Data")]
	public class SO_BatchLootBoxData : SO_ChartData
	{
		/// <summary>
		/// Lootbox object data container
		/// </summary>
		[SerializeField] private SO_LootBoxObjectDataCollection _lootBoxObjectsDataCollection;

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="jsonData"> list of item in json data </param>
		public override void Initialize(string jsonData)
        {
			var lootBoxDatas = Utility.StaticReflection.DatabaseItemsParse<LootBoxData>(jsonData); // parse json data into list of lootbox data
			
			foreach (var lootboxData in lootBoxDatas)
				_lootBoxObjectsDataCollection.GetLootBoxObjectDataByID(lootboxData.LootBoxID).SetLootboxData(lootboxData);
        }

		/// <summary>
		/// Get specific lootbox object data by id
		/// </summary>
		/// <param name="id"> target lootbox id </param>
		/// <returns> lootbox object data </returns>
		public SO_LootBoxObjectData GetLootBoxDataByID(string id)
			=> _lootBoxObjectsDataCollection.GetLootBoxObjectDataByID(id);
	}
}