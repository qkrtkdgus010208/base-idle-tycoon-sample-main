namespace Project
{
    using UnityEngine.Events;


    public static class StaticAudioEvents
    {
        /// <summary>
        /// Set volume events
        /// string ID (BGM / SFX), float volume value
        /// </summary>
        public static UnityAction<string, float> OnSetVolume;

        /// <summary>
        /// Set Audio state events
        /// turn (on / off) (true / false) BGM
        /// </summary>
        public static UnityAction<bool> SetAudioState;

        /// <summary>
        /// Play general button sound events
        /// </summary>
        public static UnityAction GeneralButtonSFX;

        /// <summary>
        /// Play general UI sound events
        /// </summary>
        public static UnityAction GeneralUISFX;

        /// <summary>
        /// Reload audio volume
        /// </summary>
        public static UnityAction ReloadAudioVolume;
    }
}
