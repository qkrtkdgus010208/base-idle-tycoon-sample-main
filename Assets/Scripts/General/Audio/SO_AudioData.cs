namespace Project
{
    using UnityEngine;
    using UnityEngine.Audio;

    [CreateAssetMenu(fileName = "Audio", menuName = "Scriptable Objects/New Audio Data")]
    public class SO_AudioData : ScriptableObject
    {
        /// <summary>
        /// Initialize issue error message
        /// </summary>
        private const string AUDIO_SOURCE_NULL_ERROR_MESSAGE = "Null Audio Source, Add Sound Data To DDOL Game Object. SO Name :";

        /// <summary>
        /// audio clip asset
        /// </summary>
        [SerializeField] private AudioClip Clip;

        /// <summary>
        /// volume override based on audio clip asset
        /// </summary>
        [Range(0, 1)] [SerializeField] private float Volume;

        /// <summary>
        /// audio looping state
        /// </summary>
        [SerializeField] private bool IsLoop;

        /// <summary>
        /// sudiosource for this audio clip
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// Initialize & assign audio source
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="audioMixer"></param>
        public void SetAudioSource(AudioSource audioSource, AudioMixerGroup audioMixer)
        {
            audioSource.playOnAwake = false;

            audioSource.clip = Clip;
            audioSource.volume = Volume;
            audioSource.loop = IsLoop;

            audioSource.outputAudioMixerGroup = audioMixer;

            this.audioSource = audioSource;
        }

        /// <summary>
        /// Play sound
        /// </summary>
        public void PlaySound()
        {
            if (audioSource == null)
                Debug.Log(AUDIO_SOURCE_NULL_ERROR_MESSAGE);

            audioSource.Play();
        }

        /// <summary>
        /// Stop sound
        /// </summary>
        public void StopSound() => audioSource.Stop();

        /// <summary>
        /// Get audio clip length (1f : 1sec)
        /// </summary>
        /// <returns></returns>
        public float AudioLength() => Clip.length;
    }
}