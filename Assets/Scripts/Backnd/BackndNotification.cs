namespace Project.BackndServer
{
	using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BackEnd;

	public static class BackndNotification
	{
		/// <summary>
		/// Message when receive new mail 
		/// </summary>
		private const string NEW_MESSAGE_STR = "New Message";

		/// <summary>
		/// Message when receive new announcement 
		/// </summary>
		private const string NEW_ANNOUNCEMENT_STR = "New Announcement";

		/// <summary>
		/// Reconnection delay in milliseconds
		/// </summary>
		private const int RECONNECTION_DELAY = 5000;

		/// <summary>
		/// Events when notifcation connected to server
		/// </summary>
		public static Action<bool, string> OnAuthorize;

		/// <summary>
		/// Events when server satus changed
		/// </summary>
		public static Action<BackEnd.Socketio.ServerStatusType> OnServerStatusChanged;

		/// <summary>
		/// Events when receiving new mail
		/// </summary>
		public static Action OnReceiveMail;

		/// <summary>
		/// Events when player disconnect
		/// </summary>
		public static Action<string> OnDisconnect;

		/// <summary>
		/// Connected state
		/// </summary>
		private static bool isConnected;

		/// <summary>
		/// Initialize state
		/// </summary>
		private static bool isInitialized;


		/// <summary>
		/// Initialize & try to connect notification to server
		/// </summary>
		public static void Initialize()
		{
			if (isInitialized)
				return;

			Backend.Notification.OnAuthorize = // Subs on connected notice
				(isSuccess, message) =>
				{
                    UnityEngine.Debug.Log("Xcute On Authorize : " + isSuccess);

					if (!isSuccess)
						NoticeUIController.Instance.ShowNotice(message, null);

					isConnected = isSuccess;
					OnAuthorize?.Invoke(isSuccess, message);
				};

			Backend.Notification.OnReceivedMessage = // Subs on receive new mail
				() =>
				{
					OnReceiveMail?.Invoke();
					NoticeUIController.Instance.ShowFlashNotice(NEW_MESSAGE_STR);
				};

			Backend.Notification.OnReceivedUserPost = // Subs on receive new user post
				() =>
				{
					OnReceiveMail?.Invoke();
					NoticeUIController.Instance.ShowFlashNotice(NEW_MESSAGE_STR);
				};

			Backend.Notification.OnNewPostCreated = // Subs on receive new post
				(BackEnd.Socketio.PostRepeatType postRepeatType, string title, string content, string author) => 
				{
					OnReceiveMail?.Invoke();
					NoticeUIController.Instance.ShowFlashNotice(NEW_MESSAGE_STR);
				};

			Backend.Notification.OnNewNoticeCreated = // Subs on new announcement or notice created
				(string title, string content) => 
				{
					NoticeUIController.Instance.ShowFlashNotice(NEW_ANNOUNCEMENT_STR);
				};

			Backend.Notification.OnServerStatusChanged = // Subs on server status changed
				(statusType) =>
				{
					UnityEngine.Debug.Log("Xcute Server status : " + statusType);

					OnServerStatusChanged?.Invoke(statusType);

					if (statusType > 0)
						NoticeUIController.Instance.ShowNotice(
							"Server status:\n" + statusType, ApplicationEvents.Instance.RestartGame);
				};

			Backend.Notification.OnDisConnect = // Subs on disconnected from server
				(message) =>
				{
					UnityEngine.Debug.Log("Xcute Disconnect:\n" + message);
					OnDisconnect?.Invoke(message);
				};

			ApplicationEvents.Instance.OnExit += Backend.Notification.DisConnect; // When app close, disconnect from server
			
			Backend.Notification.Connect();
			isInitialized = true; // set initialize state
		}
	}
}
