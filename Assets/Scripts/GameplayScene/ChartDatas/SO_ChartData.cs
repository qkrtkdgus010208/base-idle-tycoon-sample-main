namespace Project.Gameplay
{
    using UnityEngine;
	using Newtonsoft.Json;

	public abstract class SO_ChartData : ScriptableObject
	{
		/// <summary>
		/// Chart file id reference to backnd console chart
		/// </summary>
		[SerializeField] private string _chartFileID;

		/// <summary>
		/// Chart file id reference to backnd console chart
		/// </summary>
		public string ChartFileID => _chartFileID;

		/// <summary>
		/// Initialize data from json data
		/// </summary>
		/// <param name="jsonData"> json to convert to data </param>
		public abstract void Initialize(string jsonData);
	}
}
