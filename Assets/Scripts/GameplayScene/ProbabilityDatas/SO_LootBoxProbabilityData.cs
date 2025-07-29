namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Loot Box/Loot Box Probability Data")]
	public class SO_LootBoxProbabilityData : SO_ProbabilityData
	{
		/// <summary>
		/// Field in probability table data 
		/// that reference to obtained object from draw gacha
		/// </summary>
		private const string PROBABILITY_ITEM_ID_FIELD = "DressID";

		/// <summary>
		/// List of probability item
		/// </summary>
		[SerializeField] private List<ItemData> _contentDatas;

		/// <summary>
		/// List of probability item
		/// </summary>
		public ReadOnlyCollection<ItemData> ContentDatas => _contentDatas.AsReadOnly();

		/// <summary>
		/// Initialize probability data
		/// </summary>
		/// <param name="jsonData"></param>
		public override void Initialize(string jsonData)
        {
			_contentDatas = Utility.StaticReflection.DatabaseItemsParse<ItemData>(jsonData);
		}

		/// <summary>
		/// Draw gacha from lootbox probability table
		/// </summary>
		/// <param name="count"> draw gacha count </param>
		/// <param name="onGachaSuccess"> callback after successful gacha with list of string obtained items </param>
		public void DrawGacha(int count, Action<List<string>> onGachaSuccess)
		{
			BackndServer.BackndProbability.DrawGachaInProbabilityTable(
				FileID, 
				PROBABILITY_ITEM_ID_FIELD, 
				count, 
				onGachaSuccess);
		}

		/// <summary>
		/// Probability item data
		/// </summary>
		[Serializable]
		public struct ItemData
        {
			public float percent;
			public string ItemID;
        }
	}
}