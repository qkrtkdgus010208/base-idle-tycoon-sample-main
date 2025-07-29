namespace Project.Gameplay
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Upgrades/Kitchen Data/Kitchen Level Data Collection")]
	public class SO_BatchKitchenLevelDataCollection : ScriptableObject
	{
		/// <summary>
		/// List of kitchen data
		/// </summary>
		[SerializeField] private List<SO_BatchKitchenLevelData> _kitchensData;

		/// <summary>
		/// Get specific kitchen data which handle the dish id
		/// </summary>
		/// <param name="dishID"> Target dish id </param>
		/// <returns> batch of kitchen level data </returns>
		public SO_BatchKitchenLevelData GetKitchenDataByDishID(string dishID) =>
			_kitchensData.Find((x) => string.Equals(x.DishData.DishID, dishID));
	}
}