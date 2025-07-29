namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Loot Box/Loot Box Object Data Collection")]
	public class SO_LootBoxObjectDataCollection : ScriptableObject
	{
		/// <summary>
		/// List of lootbox object data
		/// </summary>
		[SerializeField] private List<SO_LootBoxObjectData> _lootBoxObjectDataCollection;

		/// <summary>
		/// Get specific lootbox object data by id
		/// </summary>
		/// <param name="lootBoxID"> target lootbox id </param>
		/// <returns> lootbox object data </returns>
		public SO_LootBoxObjectData GetLootBoxObjectDataByID(string lootBoxID)
			=> _lootBoxObjectDataCollection.Find(x => string.Equals(x.LootBoxID, lootBoxID));
	}
}
