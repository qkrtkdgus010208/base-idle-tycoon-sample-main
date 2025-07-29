namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Dress/Dress Object Data Collection")]
	public class SO_DressObjectDataCollection : ScriptableObject
	{
		/// <summary>
		/// List of dress object data
		/// </summary>
		[SerializeField] private List<SO_DressObjectData> _dressObjectDataCollection;

		/// <summary>
		/// Get dress object data by id
		/// </summary>
		/// <param name="dressID"> target dress id </param>
		/// <returns> dress object data </returns>
		public SO_DressObjectData GetDressObjectDataByID(string dressID)
			=> _dressObjectDataCollection.Find(x => string.Equals(x.DressID, dressID));
	}
}
