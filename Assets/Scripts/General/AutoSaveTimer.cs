namespace Project
{
    using System;
    using System.Collections;
    using UnityEngine;


    public class AutoSaveTimer : MonoBehaviour
    {
        /// <summary>
        /// Singleton
        /// </summary>
        private static AutoSaveTimer _instance;
        
        /// <summary>
        /// Singleton
        /// </summary>
        public static AutoSaveTimer Instance => _instance;

        /// <summary>
        /// Time to save events
        /// </summary>
        public static Action OnSaveTime;

        /// <summary>
        /// Auto save timer
        /// </summary>
        private float timerCount;

        /// <summary>
        /// One second timer
        /// </summary>
        private YieldInstruction awaitOneSec = new WaitForSeconds(1f);

        private void Awake()
        {
            if (_instance != null && _instance != this) // Checking singleton duplication
            {
                Destroy(gameObject); // Delete duplicated object
                return;
            }

            _instance = this; // Assign singleton
            DontDestroyOnLoad(gameObject); // set object as DDOL

            timerCount = 0; // set timer
        }

        private void Update()
        {
            timerCount += Time.deltaTime; // auto save timer

            if (timerCount >= Utility.StaticConstantDictionary.SEND_DATA_TIME) // When timer exceed SEND_DATA_TIME
            {
                OnSaveTime?.Invoke(); // push Save events
                timerCount -= Utility.StaticConstantDictionary.SEND_DATA_TIME; // Reset timer
            }
        }

        /// <summary>
        /// For data that needed to immediately saved
        /// For example: save new dress after open lootbox
        /// </summary>
        /// <param name="processSaveData"></param>
        public void SaveDataImmediately(Action processSaveData)
        {
            StartCoroutine(CollectDataAwait(processSaveData));
        }

        /// <summary>
        /// Waiting for additional data before saving to avoid double execution saving data
        /// </summary>
        /// <param name="processSaveData"></param>
        /// <returns></returns>
        private IEnumerator CollectDataAwait(Action processSaveData)
        {
            yield return awaitOneSec;
            processSaveData?.Invoke();
        }
    }
}