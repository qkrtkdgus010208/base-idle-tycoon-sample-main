namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    using System.Collections;
    using UnityEngine.Networking;

    public class AnnouncementManager : MonoBehaviour
	{
		/// <summary>
		/// Load image from data base url
		/// </summary>
		private const string IMAGE_URL_REQUEST = "http://upload-console.thebackend.io";

		/// <summary>
		/// Content Image min size
		/// </summary>
		private static Vector2 Icon_MinSize = new Vector2(150f, 100f);

		/// <summary>
		/// Content Image max size
		/// </summary>
		private static Vector2 Icon_MaxSize = new Vector2(450f, 300f);

		/// <summary>
		/// Announcement data
		/// </summary>
		[SerializeField] private SO_AnnouncementBatchData _announcementData;

		/// <summary>
		/// announcement panel UI
		/// </summary>
		[SerializeField] private GameObject _ui;

		/// <summary>
		/// announcement title text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _titleText;

		/// <summary>
		/// announcement content text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _contentText;

		/// <summary>
		/// announcement button label text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _actionButtonLabel;

		/// <summary>
		/// announcement image
		/// </summary>
		[SerializeField] private RawImage _image;

		/// <summary>
		/// announcement action button
		/// </summary>
		[SerializeField] private Button _actionButton;

		/// <summary>
		/// announcement close button
		/// </summary>
		[SerializeField] private Button _closeButton;

		/// <summary>
		/// togle to hide current showed announcement
		/// </summary>
		[SerializeField] private Toggle _isNeverShownAgain;

		/// <summary>
		/// Image download process await
		/// </summary>
		private Coroutine currentImageProcess;

		/// <summary>
		/// Current announcement showed idx
		/// </summary>
		private int currentAnnouncementIdx;

		/// <summary>
		/// Current announcement showed button link
		/// </summary>
		private string currentButtonActionLink;


        private void Awake()
        {
			_actionButton.onClick.AddListener(() => Application.OpenURL(currentButtonActionLink)); // set action button on click event to open current showed announcement url
			_closeButton.onClick.AddListener(CloseCurrentAnnouncement); // set close button on click event to close current showed announcement
		}

		private void Start()
		{
			if (!_announcementData.IsAlreadyShowed)
				Initialize(); // Initialize announcement data
		}

		/// <summary>
		/// Initialize announcement data
		/// </summary>
        public void Initialize()
		{
			currentAnnouncementIdx = -1;
			_announcementData.LoadData(ShowNextAnnouncementInList); // load announcement
        }

		/// <summary>
		/// Showing announcement in list
		/// </summary>
		private void ShowNextAnnouncementInList()
        {
			currentAnnouncementIdx++; // set current announcement idx

			if (currentAnnouncementIdx >= _announcementData.AnnouncementsListData.Count) // if all of list announcement data already showed
            {
				_ui.SetActive(false); // close ui
				return;
            }

			var announcement = _announcementData.AnnouncementsListData[currentAnnouncementIdx]; //get current announcement to show
			SetUIElement( // Set ui element to current announcement to show
				announcement.title, 
				announcement.content, 
				announcement.imageKey,
				announcement.linkUrl,
				announcement.linkButtonName,
				true
				);
		}

		/// <summary>
		/// Set pop up announcement ui element
		/// </summary>
		/// <param name="title"> pop up title </param>
		/// <param name="contents"> content message </param>
		/// <param name="imageKey"> image download url </param>
		/// <param name="buttonLink"> button action on click link </param>
		/// <param name="buttonText"> button action label </param>
		private void SetUIElement(
			string title,
			string contents,
			string imageKey,
			string buttonLink,
			string buttonText,
			bool isActiveToogle
			)
        {
			_titleText.SetText(title); // set title output text
			_contentText.SetText(contents); // set content message output text
			_actionButtonLabel.SetText(buttonText); // set action button label
			
			currentButtonActionLink = buttonLink; // set current action button link

			if (!string.IsNullOrEmpty(imageKey)) // if image url is not empty
			{
				if (currentImageProcess != null) // if closed announcement has download image process
					StopCoroutine(currentImageProcess); // stop closed announcement image download process to avoid override / showing wrong image

				currentImageProcess = StartCoroutine(LoadImage(imageKey)); // set current image process then process download image
            }

			_image.gameObject.SetActive(!string.IsNullOrEmpty(imageKey)); // set active image if image url is not empty
			_actionButton.gameObject.SetActive(!string.IsNullOrEmpty(buttonLink)); // set active button action link if button link is not empty

			_isNeverShownAgain.gameObject.SetActive(isActiveToogle);
			_isNeverShownAgain.SetIsOnWithoutNotify(false); // reset don't show announcement togle

			_ui.SetActive(true); // set active ui
		}

		/// <summary>
		/// Close current showed announcement
		/// </summary>
		private void CloseCurrentAnnouncement()
		{
			if (_isNeverShownAgain.isOn) // if don't show announcement togle state on 
			{
				_announcementData.HideAnnouncement(currentAnnouncementIdx); // set hide last announcement showed
				currentAnnouncementIdx--; // reduce announcement idx
			}

			ShowNextAnnouncementInList(); // showed next announcement in list
		}

		/// <summary>
		/// Process download image
		/// </summary>
		/// <param name="imageKey"> image url </param>
		/// <returns></returns>
		private IEnumerator LoadImage(string imageKey)
        {
			_image.texture = null; // reset image texture

			var imageProcess = UnityWebRequestTexture.GetTexture(IMAGE_URL_REQUEST + imageKey); // get texture process task
			yield return imageProcess.SendWebRequest(); // waiting for image process

			if (imageProcess.result == UnityWebRequest.Result.Success) // if success
			{
				_image.texture = ((DownloadHandlerTexture)imageProcess.downloadHandler).texture; // set image texture
				Project.Utility.StaticImageSpriteFitter.FitImageSprite(Icon_MinSize, Icon_MaxSize, _image); // set image size
			}
        }
	}
}
