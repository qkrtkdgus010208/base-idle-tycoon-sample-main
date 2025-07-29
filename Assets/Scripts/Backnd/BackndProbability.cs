namespace Project.BackndServer
{
	using System;
    using System.Collections.Generic;
    using BackEnd;
	using UnityEngine;


	public static class BackndProbability
	{
		/// <summary>
		/// key for item that obtained from gacha in return object
		/// </summary>
		private const string ITEM_ELEMENT_KEY = "elements";

		/// <summary>
		/// Get list of probability table from backnd console
		/// </summary>
		/// <returns> list of probability table data in backnd return object form </returns>
		public static List<BackEnd.ProbabilityContent.ProbabilityContentItem> GetProbabilityContentData()
		{
			// using CDN methode
			var getTableList = Backend.CDN.Probability.Table.Get();

			if (!getTableList.IsSuccess())
			{
				NoticeUIController.Instance.ShowNotice("Geting probability table list error :\n" + getTableList.Message, null);
				return null;
			}

			var getTableItemList = Backend.CDN.Probability.Get(getTableList.GetProbabilityTableItemList());

			if (!getTableItemList.IsSuccess())
			{
				NoticeUIController.Instance.ShowNotice("Geting probability table item list error :\n" + getTableItemList.Message, null);
				return null;
			}

			return getTableItemList.GetProbabilityContentList();
		}

		/// <summary>
		/// Draw gacha from specific probability table
		/// </summary>
		/// <param name="fileID"> target probability file id </param>
		/// <param name="drawCount"> draw gacha count </param>
		/// <param name="onGetItem"> callback after successful gacha with list of string obtained items </param>
		public static void DrawGachaInProbabilityTable(string fileID, string itemID, int drawCount, Action<List<string>> onGetItem)
        {
			Backend.Probability.GetProbabilitys(fileID, drawCount, (bro) =>
			{
				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice("Failed gacha item\nMessage: " + bro.Message, 
						() => DrawGachaInProbabilityTable(fileID, itemID, drawCount, onGetItem));
					return;
				}

				var data = bro.GetFlattenJSON()[ITEM_ELEMENT_KEY];
				var itemsID = new List<string>();

				for (int i = 0; i < data.Count; i++)
					itemsID.Add(data[i][itemID].ToString());

				onGetItem?.Invoke(itemsID);
			});
		}
	}
}
