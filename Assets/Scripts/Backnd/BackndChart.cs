namespace Project.BackndServer
{
	using System.Collections.Generic;
    using BackEnd;


	public static class BackndChart
	{
		/// <summary>
		/// Get chart data from backnd console
		/// </summary>
		/// <returns> list of chart table data in backnd return object form </returns>
		public static List<BackEnd.Content.ContentItem> GetChartDatas()
		{
			// using CDN method
			var backndGetChartTable = Backend.CDN.Content.Table.Get(); // Get all of table in chart

			if (!backndGetChartTable.IsSuccess())
			{
				NoticeUIController.Instance.ShowNotice("Load Chart Table Error :" + backndGetChartTable.Message, null);
				return null;
			}

			var backndGetChartData = Backend.CDN.Content.Get(backndGetChartTable.GetContentTableItemList()); // Get table content
			
			if (!backndGetChartData.IsSuccess())
            {
				NoticeUIController.Instance.ShowNotice("Load Chart Data Error :" + backndGetChartTable.Message, null);
				return null;
			}

			return backndGetChartData.GetContentList();
		}
	}
}
