namespace Project.Gameplay
{
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Dress/Dress Data")]
	public class SO_BatchDressData : SO_ChartData
	{
		/// <summary>
		/// Dress object data container
		/// </summary>
		[SerializeField] private SO_DressObjectDataCollection _dressObjectDataCollection;

		/// <summary>
		/// All of dress data that already transfomed into Dress object
		/// </summary>
		[SerializeField] private DressObject[] _dressCollection;
		
		/// <summary>
		/// Initialize dress data
		/// </summary>
		/// <param name="jsonData"> json chart data </param>
		public override void Initialize(string jsonData)
        {
			var dressData = Utility.StaticReflection.DatabaseItemsParse<DressData>(jsonData); // parse json data into list of dress data
			_dressCollection = new DressObject[dressData.Count];

			// transform all dress data into dress object
			for (int i = 0; i < dressData.Count; i++)
				_dressCollection[i] =
					new DressObject(
					dressData[i],
					_dressObjectDataCollection.GetDressObjectDataByID(dressData[i].DressID)
					);
        }

		/// <summary>
		/// Get dress object by id
		/// </summary>
		/// <param name="id"> target dress id </param>
		/// <returns> dress object </returns>
		public DressObject GetDressDataByID(string id)
        {
			foreach (var data in _dressCollection)
				if (string.Equals(data.DressID, id))
					return data;

            Debug.Log("Missing Dress ID :" + id);
			return null;
        }
	}
}