namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
	using Newtonsoft.Json;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Announcement/Announcement Data")]
	public class SO_AnnouncementBatchData : ScriptableObject
	{
		/// <summary>
		/// Hide announcement player prefs key
		/// </summary>
		private const string HIDE_ANNOUNCEMENT_DATA_KEY = "HideAnnouncement_inDate:{0}";
		
		/// <summary>
		/// list of announcement data
		/// </summary>
		private List<Announcement> _announcementListData = new List<Announcement>();

		/// <summary>
		/// list of announcement data
		/// </summary>
		public ReadOnlyCollection<Announcement> AnnouncementsListData => _announcementListData.AsReadOnly();

		/// <summary>
		/// Is announcement already showed state
		/// </summary>
		private bool isAlreadyShowed;
		public bool IsAlreadyShowed
        {
			get => isAlreadyShowed;
			set => isAlreadyShowed = value;
		}

		/// <summary>
		/// Load announcement data from database
		/// </summary>
		/// <param name="onSuccess"> callbacks after load announcements data success </param>
		public void LoadData(Action onSuccess)
		{
			_announcementListData.Clear(); // generate new list of announcement data

			BackndServer.BackndAnnouncement.LoadAnnouncement((json) => 
			{
				var listData = Utility.StaticReflection.DatabaseItemsParse<Announcement>(json); // convert json to list of announcement

				foreach (var data in listData)
				{
					if (PlayerPrefs.HasKey(string.Format(HIDE_ANNOUNCEMENT_DATA_KEY, data.inDate))) // checking if this announcement was hide
						continue;

					_announcementListData.Add(data); // added announcement to list
				}

				onSuccess?.Invoke();
			});
		}

		/// <summary>
		/// Set don't show again for this announcement
		/// </summary>
		/// <param name="idx"> announcement idx that will hide </param>
		public void HideAnnouncement(int idx)
        {
			var inDate = AnnouncementsListData[idx].inDate; // get announcement indate
			PlayerPrefs.SetInt(string.Format(HIDE_ANNOUNCEMENT_DATA_KEY, inDate), Utility.StaticConstantDictionary.BOOL_TO_INT_TRUE_VALUE); // save hide state to local device
			_announcementListData.Remove(_announcementListData.Find(x => string.Equals(x.inDate, inDate))); // remove from announcement list
        }

		/// <summary>
		/// Announcement backnd return object data
		/// </summary>
		public struct Announcement
		{
			/// <summary>
			/// announcement title
			/// </summary>
			public string title;

			/// <summary>
			/// announcement message
			/// </summary>
			public string content;

			/// <summary>
			/// announcement posting date
			/// </summary>
			public DateTime postingDate;

			/// <summary>
			/// announcement image url
			/// </summary>
			public string imageKey;

			/// <summary>
			/// announcement in date
			/// </summary>
			public string inDate;

			/// <summary>
			/// announcement uuid
			/// </summary>
			public string uuid;

			/// <summary>
			/// announcement button link
			/// </summary>
			public string linkUrl;

			/// <summary>
			/// announcement is public state
			/// </summary>
			public string isPublic;
			
			/// <summary>
			/// announcement button lable
			/// </summary>
			public string linkButtonName;

			/// <summary>
			/// announcement author
			/// </summary>
			public string author;
		}
	}
}
