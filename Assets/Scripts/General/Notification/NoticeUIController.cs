namespace Project
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

	public class NoticeUIController : MonoBehaviour
	{
        /// <summary>
        /// singleton
        /// </summary>
        private static NoticeUIController instance;

        /// <summary>
        /// singleton
        /// </summary>
        public static NoticeUIController Instance => instance;

        /// <summary>
        /// Notice UI
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// output text for notice message
        /// </summary>
        [SerializeField] private Text _messageText;

        /// <summary>
        /// button for close notice ui & do additional confirm events
        /// </summary>
        [SerializeField] private Button _confirmButton;




        [Space(5)]
        [Header("Flash Notif")]

        /// <summary>
        /// flash notice parent transform
        /// </summary>
        [SerializeField] private Transform _flashListParent;

        /// <summary>
        /// flash notice template
        /// </summary>
        [SerializeField] private FlashNoticeListUI _tempFlashNoticeList;

        /// <summary>
        /// flash notice pool
        /// </summary>
        private Utility.ClassPooling<FlashNoticeListUI> flashListPool;
        

        private void Awake()
        {
            if (instance != null && instance != this) // Checking singleton
            {
                Destroy(gameObject); // Destroy duplicated object
                return;
            }

            instance = this; // Assign singleton
            DontDestroyOnLoad(gameObject); // Set object as DDOL
            
            flashListPool = new Utility.ClassPooling<FlashNoticeListUI>( // Initialize flash notice pool
                () => Instantiate(_tempFlashNoticeList, _flashListParent));
        }

        /// <summary>
        /// Show notice
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="confirmButtonAction"></param>
        public void ShowNotice(string msg, Action confirmButtonAction)
        {
            _messageText.text = msg; // set notice message


            _confirmButton.onClick.RemoveAllListeners(); // clear confirm button events first
            _confirmButton.onClick.AddListener( // add confirm button events
                () => {
                    _ui.SetActive(false); // close notice UI
                    confirmButtonAction?.Invoke(); // do confirm events if not null
                }
             );

            _ui.SetActive(true); // active notice ui
        }

        /// <summary>
        /// Set flash notice
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        public void ShowFlashNotice(string message, Sprite icon = null)
        {
            var flashNotice = flashListPool.GetFromPool(); // Get inactive flash notice obj from pool

            flashNotice.SetNotice(message, icon); // Set flash notice ui elements
            flashNotice.SetActive(true); // Show flash notice
        }
    }
}
