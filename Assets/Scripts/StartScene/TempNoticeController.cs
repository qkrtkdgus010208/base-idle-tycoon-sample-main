namespace Project.StartScene
{
    using Newtonsoft.Json;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TempNoticeController : MonoBehaviour
	{
		/// <summary>
		/// announcement panel UI
		/// </summary>
		[SerializeField] private GameObject _ui;

		/// <summary>
		/// announcement content text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _contentText;

		/// <summary>
		/// confirmation button
		/// </summary>
		[SerializeField] private Button _refreshButton;


		private void Awake()
        {
            StaticStartSceneEventsManager.ShowMaintenanceNotice = ShowNotice;
			_refreshButton.onClick.AddListener(ApplicationEvents.Instance.RestartGame);
        }

		/// <summary>
		/// Showing temporary notice
		/// </summary>
		private void ShowNotice()
		{
			BackndServer.BackndAnnouncement.GetTemporaryNotice(SetActiveUI);
		}

		/// <summary>
		/// Set active 
		/// </summary>
		/// <param name="jsonData"> notice data in json form </param>
		private void SetActiveUI(string jsonData)
		{
			var tempNotice = JsonConvert.DeserializeObject<TempNotice>(jsonData);
			if (!tempNotice.isUse || string.IsNullOrEmpty(tempNotice.contents)) // if there is no temp notice to show
			{
                Debug.Log("Temporary notice is not set yet!");
				return;
			}

			_contentText.SetText(tempNotice.contents);
			_ui.SetActive(true);
		}

		/// <summary>
		/// Temp notice backnd return object data
		/// </summary>
		public struct TempNotice
		{
			/// <summary>
			/// is use state
			/// </summary>
			public bool isUse;

			/// <summary>
			/// content message
			/// </summary>
			public string contents;
		}
	}
}
