namespace Project.Gameplay
{
    using BackndServer;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System;

    public class ProfileManager : MonoBehaviour
	{
        /// <summary>
        /// Profile image data collection
        /// </summary>
        [SerializeField] private SO_ProfileImageDataCollection _profileImageDataCollection;

        /// <summary>
        /// Profile UI
        /// </summary>
        [SerializeField] private GameObject _ui;
		
        /// <summary>
        /// button to open profile UI
        /// </summary>
        [SerializeField] private Button _openProfileUIButton;

        /// <summary>
        /// button to close profile UI
        /// </summary>
		[SerializeField] private Button _closeProfileUIButton;

        /// <summary>
        /// Input field for user name
        /// </summary>
        [SerializeField] private TMP_InputField _userNameField;

        /// <summary>
        /// Profile image that user use
        /// </summary>
        [SerializeField] private Image _profileImage;

        /// <summary>
        /// Profile image in open profile UI button
        /// </summary>
        [SerializeField] private Image _buttonOpenProfileImageIcon;

        /// <summary>
        /// Text name in open profile UI button
        /// </summary>
        [SerializeField] private TextMeshProUGUI _buttonOpenProfileUserNameText;



        /// <summary>
        /// user name confirmation UI
        /// </summary>
        [Space(20)]
        [Header("Confirmation UI")]
        [SerializeField] private GameObject _userNameConfirmationUI;

        /// <summary>
        /// Button to confirm user name
        /// </summary>
        [SerializeField] private Button _confirmUserNameConfirmationButton;

        /// <summary>
        /// Button to cancel user name
        /// </summary>
        [SerializeField] private Button _cancelUserNameConfirmationButton;

        /// <summary>
        /// inputed user name output text
        /// </summary>
        [SerializeField] private TextMeshProUGUI _userNameOutputText;


        /// <summary>
        /// profile image selection UI
        /// </summary>
        [Space(20)]
        [Header("Profile Image Choose")]
        [SerializeField] private GameObject _profileImageSelectionUI;

        /// <summary>
        /// button to open selection profile image UI
        /// </summary>
        [SerializeField] private Button _openProfileImageSelectionUI;

        /// <summary>
        /// button to close selection profile image UI
        /// </summary>
        [SerializeField] private Button _closeProfileImageSelectionUI;

        /// <summary>
        /// profile image list UI template
        /// </summary>
        [SerializeField] private ProfileImageListUI _pofileImageListUITemplate;

        /// <summary>
        /// profile image list UI parent
        /// </summary>
        [SerializeField] private Transform _profileImageListUIParent;

        /// <summary>
        /// Events to open profile ui
        /// </summary>
        public static Action OpenProfileUI;

        /// <summary>
        /// profile image ui that handle current used profile image
        /// </summary>
        private ProfileImageListUI currentUsedProfileImage;

        /// <summary>
        /// username that user input
        /// </summary>
        private string inputUserName;

        /// <summary>
        /// current used profile image id
        /// </summary>
        private string usedProfileImageID;

        /// <summary>
        /// is null user name in user account state
        /// </summary>
        private bool isNullUserName;

        private void Awake()
        {
            OpenProfileUI = () => SetActiveUI(true);

            _openProfileUIButton.onClick.AddListener(() => SetActiveUI(true));
            _closeProfileUIButton.onClick.AddListener(() => SetActiveUI(false));

            _userNameField.onEndEdit.AddListener(SetUserName);

            _confirmUserNameConfirmationButton.onClick.AddListener(ConfirmUserName);
            _cancelUserNameConfirmationButton.onClick.AddListener(() => _userNameConfirmationUI.SetActive(false));

            _openProfileImageSelectionUI.onClick.AddListener(() => SetActiveProfileImageSelectionUI(true));
            _closeProfileImageSelectionUI.onClick.AddListener(() => SetActiveProfileImageSelectionUI(false));

            SetNicknameFieldUIElements();
            SetProfileImageUIElements();

            InitializeProfileImageList();
        }

        /// <summary>
        /// Set active profile UI
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActiveUI(bool state)
        {
            if (!state && isNullUserName)
            {
                _userNameField.SetTextWithoutNotify(string.Empty);
                _userNameConfirmationUI.SetActive(false);
                SetActiveProfileImageSelectionUI(false);
            }

            _ui.SetActive(state);
        }

        /// <summary>
        /// Set user name
        /// </summary>
        /// <param name="userName"> user inputed user name </param>
        private void SetUserName(string userName)
        {
            if (!BackndUserInfo.Instance.CheckingNameValidation(userName))
                return;

            inputUserName = userName;

            _userNameOutputText.SetText(inputUserName);
            _userNameConfirmationUI.SetActive(true);
        }

        /// <summary>
        /// Confirmation user name
        /// </summary>
        private void ConfirmUserName()
        {
            if (!BackndUserInfo.Instance.SetUserName(inputUserName))
                return;

            NoticeUIController.Instance.ShowFlashNotice(inputUserName);
            SetNicknameFieldUIElements();

            _userNameConfirmationUI.SetActive(false);
        }

        /// <summary>
        /// Set nickname input field ui elements
        /// </summary>
        private void SetNicknameFieldUIElements()
        {
            string userName = BackndUserInfo.Instance.UserInfo.nickname;

            isNullUserName = string.IsNullOrEmpty(userName);

            if (!isNullUserName)
            {
                _userNameField.SetTextWithoutNotify(userName);
                _buttonOpenProfileUserNameText.SetText(userName);
            }

            _userNameField.interactable = isNullUserName;
            _userNameField.readOnly = !isNullUserName;
        }

        /// <summary>
        /// Set active profile selection UI
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveProfileImageSelectionUI(bool state)
        {
            _profileImageSelectionUI.SetActive(state);
        }

        /// <summary>
        /// Initialize profile image data
        /// </summary>
        private void InitializeProfileImageList()
        {
            foreach (var profileImageData in _profileImageDataCollection.ProfileImageDataCollection)
            {
                var ui = Instantiate(_pofileImageListUITemplate, _profileImageListUIParent);
                ui.Initialize(profileImageData, SetProfileImage);

                if (string.Equals(profileImageData.ProfileImageID, usedProfileImageID) ||
                   (string.IsNullOrEmpty(usedProfileImageID) && currentUsedProfileImage == null))
                {
                    ui.SetUsedUIElements(true);
                    currentUsedProfileImage = ui;
                }
                else
                    ui.SetUsedUIElements(false);
            }
        }

        /// <summary>
        /// Update current used profile image
        /// </summary>
        /// <param name="profileImageUI"> profile image ui that handle current used profile image </param>
        /// <param name="profileImageData"> profile image data </param>
        private void SetProfileImage(ProfileImageListUI profileImageUI, SO_ProfileImageData profileImageData)
        {
            currentUsedProfileImage.SetUsedUIElements(false);
            currentUsedProfileImage = profileImageUI;
            currentUsedProfileImage.SetUsedUIElements(true);

            SaveData.UserDataManager.Instance.UpdatePlayerUsedProfileImageID(profileImageData.ProfileImageID);
            SetProfileImageUIElements();
        }

        /// <summary>
        /// Set profile image ui elements
        /// </summary>
        private void SetProfileImageUIElements()
        {
            usedProfileImageID = SaveData.UserDataManager.Instance.UsedProfileImageID;

            _profileImage.sprite = string.IsNullOrEmpty(usedProfileImageID) ? 
                _profileImageDataCollection.GetProfileImageDefaultData().ProfileImageSprite : 
                _profileImageDataCollection.GetProfileImageDataByID(usedProfileImageID).ProfileImageSprite;

            _buttonOpenProfileImageIcon.sprite = _profileImage.sprite;
        }
    }
}
