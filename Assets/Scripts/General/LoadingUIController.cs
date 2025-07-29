namespace Project
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections;

    public class LoadingUIController : MonoBehaviour
	{
        /// <summary>
        /// Singleton
        /// </summary>
        private static LoadingUIController instance;

        /// <summary>
        /// Singleton
        /// </summary>
        public static LoadingUIController Instance => instance;

        /// <summary>
        /// Loading UI
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// Loading progression bar UI
        /// </summary>
        [SerializeField] private Slider _progressBar;

        /// <summary>
        /// Loading info description text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _loadingInfoDescription;

        /// <summary>
        /// Waiting time before close loading UI
        /// </summary>
        private YieldInstruction _awaitLoadingUIClose = new WaitForSeconds(1f);

        /// <summary>
        /// Load data in process count in current session
        /// </summary>
        private int currentLoadingCount;

        private void Awake()
        {
            if (instance != null && instance != this) // Checking for duplication singleton
            {
                Destroy(gameObject); // Delete duplication obeject
                return;
            }

            instance = this; // Assign singleton
            DontDestroyOnLoad(gameObject); // Set object as DDOL
        }

        /// <summary>
        /// Show loading, increase loading in process count
        /// </summary>
        public void Loading(string loadingInfo = "")
        {
            if (currentLoadingCount == 0) // Active loading UI if there is no load in process in this session yet before
                _ui.SetActive(true); 

            if (!string.IsNullOrEmpty(loadingInfo)) // if there is loading description
                _loadingInfoDescription.SetText(loadingInfo); // Set loading description text output

            currentLoadingCount++; // Increase loading in process
            _progressBar.maxValue = currentLoadingCount; // Set progression bar UI complete value
        }


        /// <summary>
        /// Decrease loading in process,
        /// close loading UI after all loading in process completed
        /// </summary>
        public void FinishLoading()
        {
            currentLoadingCount--; // Decrease loading in process
            _progressBar.value++; // Increase progression bar

            if (currentLoadingCount <= 0) // when there is no loading in process
                StartCoroutine(CloseLoadingAwait()); // Await befor close loading UI
        }

        /// <summary>
        /// Await before close loading UI
        /// To make sure all data loaded in this session
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseLoadingAwait()
        {
            yield return _awaitLoadingUIClose; // Await

            if (currentLoadingCount <= 0) // Make sure there is no loading in process
            {
                _ui.SetActive(false); // Close loading UI

                currentLoadingCount = 0; // reset current loading count session
                _progressBar.value = 0; // reset loading bar progression
            }
        }
    }
}
