namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Upgrades/Kitchen Data/Kitchen Level Data")]
	public class SO_BatchKitchenLevelData : SO_ChartData
	{
		/// <summary>
		/// List of kitchen level data
		/// </summary>
		[SerializeField] private List<KitchenLevelData> _kitchenLevelDatas;

		/// <summary>
		/// Dish data that this kitchen station handle
		/// </summary>
		[SerializeField] private SO_DishData _dishData;

		/// <summary>
		/// Dish data that this kitchen station handle
		/// </summary>
		public SO_DishData DishData => _dishData;

		/// <summary>
		/// Get this kitchen station maximal level
		/// </summary>
		public int MaxLevel => _kitchenLevelDatas[^1].Level;

		/// <summary>
		/// Initialize kitchen level data
		/// </summary>
		/// <param name="jsonData"></param>
		public override void Initialize(string jsonData)
        {
			_kitchenLevelDatas = Utility.StaticReflection.DatabaseItemsParse<KitchenLevelData>(jsonData); // parse json data into list of kitchen level data
		}

		/// <summary>
		/// Get kitchen level data at specific level
		/// </summary>
		/// <param name="level"> level target </param>
		/// <returns> kitchen level data </returns>
        public KitchenLevelData GetLevelData(int level)
        {
			foreach (KitchenLevelData levelData in _kitchenLevelDatas)
				if (levelData.Level == level)
					return levelData;
				else if (levelData.Level > level)
					break;

			return new KitchenLevelData();
        }
	}
}