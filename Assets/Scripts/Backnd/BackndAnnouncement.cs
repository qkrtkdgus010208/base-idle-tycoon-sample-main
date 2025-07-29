namespace Project.BackndServer
{
	using BackEnd;
	using System;

	public static class BackndAnnouncement
	{
		/// <summary>
		/// Load announcement from backnd console
		/// </summary>
		/// <param name="onNoticeLoaded"> callback after successful get the announcement data </param>
		public static void LoadAnnouncement(Action<string> onNoticeLoaded)
		{
			Backend.Notice.NoticeList(bro =>
			{
				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice("Load Notice List Failed:\n" + bro.Message, null);
					return;
				}

				onNoticeLoaded?.Invoke(bro.FlattenRows().ToJson());
			});
		}

		/// <summary>
		/// Load temporary notice from backnd console
		/// </summary>
		/// <param name="onTempNoticeLoaded"> callback after successful get the temporary notice data </param>
		public static void GetTemporaryNotice(Action<string> onTempNoticeLoaded)
		{
			Backend.Notice.GetTempNotice((bro) => onTempNoticeLoaded?.Invoke(bro));
		}
	}
}
