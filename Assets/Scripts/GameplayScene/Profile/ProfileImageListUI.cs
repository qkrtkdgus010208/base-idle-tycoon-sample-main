namespace Project.Gameplay
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;


	public class ProfileImageListUI : MonoBehaviour
	{
		/// <summary>
		/// Button to select this profile
		/// </summary>
		[SerializeField] private Button _selectProfileButton;

		/// <summary>
		/// profile image
		/// </summary>
		[SerializeField] private Image _profileImage;

		/// <summary>
		/// Initialize profile list ui
		/// </summary>
		/// <param name="_profileImageData"> profile image data </param>
		/// <param name="onSelect"> select button on click action </param>
		public void Initialize(SO_ProfileImageData _profileImageData, Action<ProfileImageListUI, SO_ProfileImageData> onSelect)
        {
			_profileImage.sprite = _profileImageData.ProfileImageSprite;
			_selectProfileButton.onClick.AddListener(() => onSelect?.Invoke(this, _profileImageData));
        }

		/// <summary>
		/// Set ui element
		/// </summary>
		/// <param name="isUsed"> true: profile in used / false: profile can be use </param>
		public void SetUsedUIElements(bool isUsed)
        {
			_selectProfileButton.interactable = !isUsed;
		}
	}
}
