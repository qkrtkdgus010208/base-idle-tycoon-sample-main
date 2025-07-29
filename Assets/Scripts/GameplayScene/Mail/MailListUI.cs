namespace Project.Gameplay
{
	using System;
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    

    public class MailListUI : MonoBehaviour
	{
		/// <summary>
		/// default mail icon sprite
		/// </summary>
		[SerializeField] private Sprite _mailIcons;

		/// <summary>
		/// opened mail icon sprite
		/// </summary>
		[SerializeField] private Sprite _openedMailIcons;

		/// <summary>
		/// default base frame mail
		/// </summary>
		[SerializeField] private Sprite _baseClosedMail;

		/// <summary>
		/// opened mail base frame
		/// </summary>
		[SerializeField] private Sprite _baseOpenedMail;

		/// <summary>
		/// Image for mail icon
		/// </summary>
		[SerializeField] private Image _mailIcon;

		/// <summary>
		/// Mail title text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _mailTitle;
		
		/// <summary>
		/// button to open this mail
		/// </summary>
		[SerializeField] private Button openMailButton;

		/// <summary>
		/// List of image for reward icon
		/// </summary>
		[SerializeField] private Image[] _rewardIcons;

		/// <summary>
		/// Mail data
		/// </summary>
		private SO_MailData.UPostItem _mailData;

		/// <summary>
		/// Mail data
		/// </summary>
		public SO_MailData.UPostItem MailData => _mailData;

		/// <summary>
		/// Set ui elements
		/// </summary>
		/// <param name="mailData"> mail data </param>
		/// <param name="openMailAction"> on clicked open mail action </param>
		public void SetMailElement(
			SO_MailData.UPostItem mailData, 
			Action<MailListUI> openMailAction)
		{
			_mailData = mailData;
			_mailTitle.SetText(_mailData.title);

			int showIconCount = Mathf.Min(_mailData.items.Count, _rewardIcons.Length);

			for (int i = 0; i < showIconCount; i++)
			{
				_rewardIcons[i].sprite = 
					Utility.StaticImageDictionary.Instance.GetImageSpriteByID(_mailData.items[i].item.itemID);

				_rewardIcons[i].transform.parent.gameObject.SetActive(true);
			}

			openMailButton.onClick.AddListener(() => openMailAction(this));
		}

		/// <summary>
		/// Set active mail in list
		/// </summary>
		/// <param name="state"> true: set active in list / false: deactive from list </param>
		public void SetActiveUI(bool state)
        {
			gameObject.SetActive(state);
        }
	}
}
