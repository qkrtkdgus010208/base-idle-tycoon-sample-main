namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Shop/Lootbox Sale Data")]
	public class SO_BatchLootboxSaleData : SO_ChartData
	{
		/// <summary>
		/// List of lootbox sale in shop
		/// </summary>
		[SerializeField] private List<LootboxSaleData> _lootboxSaleDatas;

		/// <summary>
		/// List of lootbox sale in shop
		/// </summary>
		public ReadOnlyCollection<LootboxSaleData> LootboxSaleDatas => _lootboxSaleDatas.AsReadOnly();

		/// <summary>
		/// Initialize lootbox sale data
		/// </summary>
		/// <param name="jsonData"> json data </param>
		public override void Initialize(string jsonData)
		{
			_lootboxSaleDatas = Utility.StaticReflection.DatabaseItemsParse<LootboxSaleData>(jsonData); // Parse json data into list of lootbox sale data
		}
	}
}