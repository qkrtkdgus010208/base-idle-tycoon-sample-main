namespace Project.BackndServer
{
	using BackEnd;
	using Newtonsoft.Json;

	public class BackndUserInfo
	{
		/// <summary>
		/// Max characater inputed for username
		/// </summary>
		private const int MAX_CHARACTER_IN_NAME = 20;

		/// <summary>
		/// User info date
		/// </summary>
		private UserInfoData _userInfo;

		/// <summary>
		/// User info date
		/// </summary>
		public UserInfoData UserInfo => _userInfo;

		/// <summary>
		/// is user info loaded state
		/// </summary>
		private bool isDataLoaded;

		/// <summary>
		/// is user info loaded state
		/// </summary>
		public bool IsDataLoaded => isDataLoaded;

		/// <summary>
		/// Singleton
		/// </summary>
		private static BackndUserInfo _instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static BackndUserInfo Instance
		{
			get
			{
				if (_instance == null)
					_instance = new BackndUserInfo();
				return _instance;
			}
		}

		/// <summary>
		/// Checking is the account is guest account
		/// </summary>
		public bool IsGuestAccount => string.IsNullOrEmpty(_userInfo.federationId);

		/// <summary>
		/// Load user info data
		/// </summary>
		public void LoadData()
        {
			LoadingUIController.Instance.Loading();
				
			var bro = Backend.BMember.GetUserInfo();
			
			if (!bro.IsSuccess())
            {
				NoticeUIController.Instance.ShowNotice("Failed Get User Info\nMessage: " + bro.Message, LoadData);
				isDataLoaded = false;

				LoadingUIController.Instance.FinishLoading();
				return;
            }

			_userInfo = JsonConvert.DeserializeObject<UserInfoData>(bro.GetReturnValuetoJSON()["row"].ToJson());
			isDataLoaded = bro.IsSuccess();

			LoadingUIController.Instance.FinishLoading();
		}

		/// <summary>
		/// Set user name
		/// </summary>
		/// <param name="userName"></param>
		/// <returns> state if the name successful to use </returns>
		public bool SetUserName(string userName)
        {
			if (!CheckingNameValidation(userName))
				return false;

			var bro = Backend.BMember.UpdateNickname(userName);

			if (!bro.IsSuccess())
				NoticeUIController.Instance.ShowNotice("Change Name Failed\nMessage: " + bro.Message, null);
			else
				_userInfo.nickname = userName;

			return bro.IsSuccess();
		}

		/// <summary>
		/// Check is name that user inputed valid
		/// </summary>
		/// <param name="userName"> user name </param>
		/// <returns> state is name valid / available to use </returns>
		public bool CheckingNameValidation(string userName)
        {
			if (string.IsNullOrEmpty(userName))
			{
				NoticeUIController.Instance.ShowNotice("Name cannot be empty.", null);
				return false;
			}

			if (userName.Length > MAX_CHARACTER_IN_NAME)
			{
				NoticeUIController.Instance.ShowNotice(string.Format("Name cannot be longer than {0} characters.", MAX_CHARACTER_IN_NAME), null);
				return false;
			}

			if (userName.Contains(' '))
            {
				NoticeUIController.Instance.ShowNotice("Name cannot contain spaces.", null);
				return false;
			}

			var bro = Backend.BMember.CheckNicknameDuplication(userName);
			if (!bro.IsSuccess())
			{
				if (bro.StatusCode == 409)
					NoticeUIController.Instance.ShowNotice("Oops! That name is already in use.", null);
				else
					NoticeUIController.Instance.ShowNotice("Change Name Failed\nMessage: " + bro.Message, null);

				return false;
			}
			
			return true;
		}

		/// <summary>
		/// User info data
		/// </summary>
		[System.Serializable]
		public struct UserInfoData
        {
            public string gamerId;
            public string countryCode;
            public string nickname;
            public string inDate;
            public string emailForFindPassword;
            public string subscriptionType;
            public string federationId;
        }
	}
}
