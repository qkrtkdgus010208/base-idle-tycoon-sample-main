namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Shop/Gems Sale Data")]
	public class SO_BatchGemsSaleData : SO_ChartData
	{
		/// <summary>
		/// List of gems sale in shop
		/// </summary>
		[SerializeField] private List<GemsSaleData> _gemsSaleDatas;

		/// <summary>
		/// List of gems sale in shop
		/// </summary>
		public ReadOnlyCollection<GemsSaleData> GemsSaleDatas => _gemsSaleDatas.AsReadOnly();

		/// <summary>
		/// Initialize gems sale data
		/// </summary>
		/// <param name="jsonData"> json data </param>
		public override void Initialize(string jsonData)
		{
			_gemsSaleDatas = Utility.StaticReflection.DatabaseItemsParse<GemsSaleData>(jsonData); // parse json data into list of gems sale data
		}
	}
}