namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;


	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Shop/Bundle Sale Data")]
	public class SO_BatchBundleSaleData : SO_ChartData
	{
		/// <summary>
		/// Bundle item code separator
		/// </summary>
		private const char STR_ITEM_KEY_SEPARATOR = '_';

		/// <summary>
		/// List of bundles data that sale in shop
		/// </summary>
		[SerializeField] private List<BundleSaleData> _bundleDatas;

		/// <summary>
		/// List of bundles data that sale in shop
		/// </summary>
		public ReadOnlyCollection<BundleSaleData> BundleDatas => _bundleDatas.AsReadOnly();

		/// <summary>
		/// Initialize bundle sale data
		/// </summary>
		/// <param name="jsonData"></param>
		public override void Initialize(string jsonData)
		{
			_bundleDatas = Utility.StaticReflection.DatabaseItemsParse<BundleSaleData>(jsonData); // parse json data into list of bundle data

			for (int i = 0; i < _bundleDatas.Count; i++)
			{
				var containItemsData = Utility.StaticReflection.DatabaseItemsParse<string>(_bundleDatas[i].ContainsItems); // parse bundles contains item (json) into list of item in bundles
				var containsItemObjects = new AbstractShopItemReceive[containItemsData.Count];

				for (int j = 0; j < containItemsData.Count; j++)
				{
					var itemStr = containItemsData[j].Split(STR_ITEM_KEY_SEPARATOR);

					containsItemObjects[j] = itemStr[0] switch
					{
						ShopItemCoin.STR_ITEM_KEY => new ShopItemCoin(itemStr[1]),
						ShopItemGems.STR_ITEM_KEY => new ShopItemGems(itemStr[1]),
						ShopItemLootBox.STR_ITEM_KEY => new ShopItemLootBox(itemStr[1], itemStr[2]),
					};
				}

				_bundleDatas[i] = new BundleSaleData() { 
					BundleID = _bundleDatas[i].BundleID,
					BundleName  = _bundleDatas[i].BundleName,
					ContainsItems = _bundleDatas[i].ContainsItems,
					ContainsItemsObjects = containsItemObjects,
				};
			}
		}
	}
}