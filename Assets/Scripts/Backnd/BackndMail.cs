namespace Project.BackndServer
{
	using System;
	using BackEnd;

	public static class BackndMail
	{
		/// <summary>
		/// key for mail content in return object
		/// </summary>
		private const string MAIL_CONTENT_KEY = "postList";

		/// <summary>
		/// key for mail item to claim in return object
		/// </summary>
		private const string MAIL_CLAIM_DATA_KEY = "postItems";

		/// <summary>
		/// Load mail from backnd console
		/// </summary>
		/// <param name="postType"> post type (User, Admin, Leaderboard, etc) </param>
		/// <param name="onMailLoaded"> callback after successful get the mail data </param>
		public static void LoadMailData(PostType postType, Action<string> onMailLoaded)
		{
			Backend.UPost.GetPostList(postType, (bro) => {
			
				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice("Load " + postType + " Mail Data Failed :\n" + bro.Message, null);
					return;
				}

				onMailLoaded?.Invoke(bro.GetReturnValuetoJSON()[MAIL_CONTENT_KEY].ToJson());
			});
		}

		/// <summary>
		/// Claim mail
		/// </summary>
		/// <param name="postType"> post type (User, Admin, Leaderboard, etc) </param>
		/// <param name="mailID"> target mail in date </param>
		/// <param name="onMailClaimed"> callback after successful claim mail </param>
		public static void ClaimMail(PostType postType, string mailID, Action<string> onMailClaimed)
		{
			Backend.UPost.ReceivePostItem(postType, mailID, (bro) => {
				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice("Claim Mail Failed " + mailID + ":\n" + bro.Message, null);
					return;
				}

				onMailClaimed?.Invoke(bro.GetReturnValuetoJSON()[MAIL_CLAIM_DATA_KEY].ToJson());
			});
		}

		/// <summary>
		/// Delete mail
		/// </summary>
		/// <param name="inDate"> target mail in date </param>
		/// <param name="onMailDeleted"> callback after successful deleted mail </param>
		public static void DeleteMail(string inDate, Action onMailDeleted)
        {
			Backend.UPost.DeleteUserPost(inDate, (bro)=>
			{
				if (!bro.IsSuccess())
				{
					NoticeUIController.Instance.ShowNotice("Delete Mail Data Failed :" + bro.Message, null);
					return;
				}

				onMailDeleted?.Invoke();
			});
		}
	}
}
