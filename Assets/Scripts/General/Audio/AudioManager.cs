namespace Project
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// BGM volume ID
        /// </summary>
        public const string BGM_VOLUME_SETTING_KEY = "VolumeBGM";

        /// <summary>
        /// SFX volume ID
        /// </summary>
        public const string SFX_VOLUME_SETTING_KEY = "VolumeSFX";

        /// <summary>
        /// Player prefs save data key
        /// </summary>
        private const string SAVE_VOLUME_DATA_KEY = "Data_Volume_";

        /// <summary>
        /// Volume to audio mixer settings multiply
        /// </summary>
        private const float SLIDER_VOLUME_TO_MIXER_MULTIPLY = 20f;

        /// <summary>
        /// singleton
        /// </summary>
        private static AudioManager _instance;

        /// <summary>
        /// singleton
        /// </summary>
        public static AudioManager Instance => _instance;

        /// <summary>
        /// SFX audio clip data collection
        /// </summary>
        [SerializeField] private SO_AudioData[] _sfx;

        /// <summary>
        /// BGM audio clip data collection
        /// </summary>
        [SerializeField] private SO_AudioData[] _bgm;

        /// <summary>
        /// bgm audio mixer
        /// </summary>
        [SerializeField] private AudioMixerGroup _bgmAudioMixer;

        /// <summary>
        /// sfx audio mixer
        /// </summary>
        [SerializeField] private AudioMixerGroup _sfxAudioMixer;

        /// <summary>
        /// audio mixer output
        /// </summary>
        [SerializeField] private AudioMixer _audioMixerSetting;

        /// <summary>
        /// BGM audio source
        /// </summary>
        [SerializeField] private AudioSource _bgmAudioSource;




        [Space(10)]
        [Header("General SFX")]

        /// <summary>
        /// General Button click SFX
        /// </summary>
        [SerializeField] private SO_AudioData _clickButtonSFX;

        /// <summary>
        /// General UI SFX
        /// </summary>
        [SerializeField] private SO_AudioData _uiOpenSFX;

        /// <summary>
        /// Current BGM volume
        /// </summary>
        private float currentBGMVolume;

        /// <summary>
        /// Current BGM volume
        /// </summary>
        public float GetBGMVolume => currentBGMVolume;
        
        /// <summary>
        /// Current sfx volume
        /// </summary>
        private float currentSFXVolume;

        /// <summary>
        /// Current sfx volume
        /// </summary>
        public float GetSFXVolume => currentSFXVolume;

        /// <summary>
        /// BGM audio state (true / false) : (on / off)
        /// </summary>
        private bool isAudioOn;

        private void Awake()
        {
            if (_instance != null && _instance != this) // Checking duplication singleton
            {
                Destroy(gameObject); // Deleted duplication
                return;
            }

            _instance = this; // Assign singleton
            DontDestroyOnLoad(gameObject); // Set object as DDOL
        }

        private void Start()
        {
            Initialize(); // Initialize
        }

        /// <summary>
        /// Initialize Audio
        /// </summary>
        private void Initialize()
        {
            foreach (SO_AudioData sound in _sfx) // Initialize all audio data in _sfx
                sound.SetAudioSource(gameObject.AddComponent<AudioSource>(), _sfxAudioMixer);

            foreach (SO_AudioData bgm in _bgm) // Initialize all audio data in _bgm
                bgm.SetAudioSource(_bgmAudioSource, _bgmAudioMixer);

            StaticAudioEvents.SetAudioState += SetAudioState; // Subscribe set audio state events
            StaticAudioEvents.OnSetVolume += SetAudioVolume; // Subscribe set audio volume events
            StaticAudioEvents.GeneralButtonSFX += () => _clickButtonSFX.PlaySound(); // Subscribe play general button SFX events
            StaticAudioEvents.GeneralUISFX += () => _uiOpenSFX.PlaySound(); // Subscribe play general UI SFX events
            StaticAudioEvents.ReloadAudioVolume += LoadAudioVolume;

            LoadAudioVolume();
        }

        private void LoadAudioVolume()
        {
            currentBGMVolume = PlayerPrefs.GetFloat(SAVE_VOLUME_DATA_KEY + BGM_VOLUME_SETTING_KEY, 1f); // Load BGM volume from local device, 1f as default
            _audioMixerSetting.SetFloat(BGM_VOLUME_SETTING_KEY, Mathf.Log10(currentBGMVolume) * SLIDER_VOLUME_TO_MIXER_MULTIPLY); // Set volume to audio mixer output

            currentSFXVolume = PlayerPrefs.GetFloat(SAVE_VOLUME_DATA_KEY + SFX_VOLUME_SETTING_KEY, 1f); // Load SFX volume from local device, 1f as default
            _audioMixerSetting.SetFloat(SFX_VOLUME_SETTING_KEY, Mathf.Log10(currentSFXVolume) * SLIDER_VOLUME_TO_MIXER_MULTIPLY); // Set volume to audio mixer output

            isAudioOn = false;
        }

        /// <summary>
        /// Set volume
        /// string (BGM / SFX) ID, float volume value
        /// </summary>
        /// <param name="audioMixerParameterName"> string (BGM / SFX) ID </param>
        /// <param name="volume"> float volume value </param>
        private void SetAudioVolume(string audioMixerParameterName, float volume)
        {
            if (string.Equals(audioMixerParameterName, BGM_VOLUME_SETTING_KEY))
                currentBGMVolume = volume;
            else if (string.Equals(audioMixerParameterName, SFX_VOLUME_SETTING_KEY))
                currentSFXVolume = volume;

            _audioMixerSetting.SetFloat(audioMixerParameterName, Mathf.Log10(volume) * SLIDER_VOLUME_TO_MIXER_MULTIPLY); // Set volume to audio mixer output
            PlayerPrefs.SetFloat(SAVE_VOLUME_DATA_KEY + audioMixerParameterName, volume); // Save volume to local device
        }

        /// <summary>
        /// Set Audio state
        /// turn (on / off) (true / false) BGM
        /// </summary>
        /// <param name="state"> turn (on / off) (true / false) BGM </param>
        private void SetAudioState(bool state)
        {
            if (isAudioOn == state) // checking state to avoid restart BGM
                return;

            isAudioOn = state; // set audio state

            if (isAudioOn)
                _bgmAudioSource.Play(); // turn on BGM
            else
                _bgmAudioSource.Stop(); // turn off BGM
        }
    }
}