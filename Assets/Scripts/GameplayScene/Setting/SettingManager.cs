namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System;

    public class SettingManager : MonoBehaviour
	{
		/// <summary>
		/// Setting ui
		/// </summary>
		[SerializeField] private GameObject _ui;

		/// <summary>
		/// Button to open setting ui
		/// </summary>
		[SerializeField] private Button _openSettingButton;

		/// <summary>
		/// Button to close setting ui
		/// </summary>
		[SerializeField] private Button _closeSettingButton;

		/// <summary>
		/// Button to google login
		/// </summary>
		[SerializeField] private Button _googleLoginButton;

		/// <summary>
		/// Button to delete data
		/// </summary>
		[SerializeField] private Button _deleteDataButton;

		/// <summary>
		/// Button to manual save
		/// </summary>
		[SerializeField] private Button _saveDataButton;

		/// <summary>
		/// slider to set bgm volume
		/// </summary>
		[SerializeField] private Slider _bgmVolumeSetting;

		/// <summary>
		/// slider to set sfx volume
		/// </summary>
		[SerializeField] private Slider _sfxVolumeSetting;

		/// <summary>
		/// app version text
		/// </summary>
		[SerializeField] private TextMeshProUGUI _gameVersionText;

		/// <summary>
		/// player uid text
		/// </summary>
		[SerializeField] private TextMeshProUGUI _playerUIDText;

		/// <summary>
		/// events to disable google login button
		/// </summary>
		public static Action SetGoogleLoginButtonMute;

        private void Awake()
        {
			SetGoogleLoginButtonMute = () => _googleLoginButton.interactable = false;

			_openSettingButton.onClick.AddListener(() => SetActiveUI(true)); // Asign open settings UI button events
			_closeSettingButton.onClick.AddListener(() => SetActiveUI(false)); // Asign close settings UI button events

			_bgmVolumeSetting.onValueChanged.AddListener((volume) => SetVolume(AudioManager.BGM_VOLUME_SETTING_KEY, volume)); // Asign bgm volume slider events
			_sfxVolumeSetting.onValueChanged.AddListener((volume) => SetVolume(AudioManager.SFX_VOLUME_SETTING_KEY, volume)); // Asign sfx volume slider events

			_gameVersionText.SetText("App Version: " + Application.version);
			_playerUIDText.SetText(BackndServer.BackndUserInfo.Instance.UserInfo.gamerId); // show player uid on settings UI

			_saveDataButton.onClick.AddListener(ApplicationEvents.Instance.SaveAllProgression); // Asign save data button events
			_deleteDataButton.onClick.AddListener(ApplicationEvents.Instance.PlayerWantToDeleteData);
			_googleLoginButton.onClick.AddListener(ApplicationEvents.Instance.ChangeToGoogleLogin); // Asign google login button events


            _googleLoginButton.interactable = BackndServer.BackndUserInfo.Instance.IsGuestAccount; // Set button active if player login as guest
        }

		/// <summary>
		/// Set Volume Action
		/// Slider volume interactable events
		/// </summary>
		/// <param name="audioMixerName"> audioMixerID </param>
		/// <param name="volume"> volume range (0.0001 ~ 1) set on slider components </param>
		private void SetVolume(string audioMixerName, float volume)
			=> StaticAudioEvents.OnSetVolume?.Invoke(audioMixerName, volume);

		/// <summary>
		/// Set Active Settings UI
		/// </summary>
		/// <param name="state"> true: set active / false: deactive </param>
		public void SetActiveUI(bool state)
		{
			_ui.SetActive(state); // Set Active UI

			if (state) // when ui set to active
			{
				_bgmVolumeSetting.SetValueWithoutNotify(AudioManager.Instance.GetBGMVolume); // Set bgm volume slider value
				_sfxVolumeSetting.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume); // Set sfx volume slider value
			}
		}
	}
}
