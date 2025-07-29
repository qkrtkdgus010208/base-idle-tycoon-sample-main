namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
	using TMPro;
    using Project.Gameplay.SaveData;

    public class DeleteAccountGameData : MonoBehaviour
    {
        /// <summary>
        /// Invalid confirmation text type input message
        /// </summary>
        private const string INVALID_CONFIRMATION_TEXT_MESSAGE = "Invalid Confirmation Text";

        /// <summary>
        /// type to confirmation delete data message
        /// </summary>
        private const string DELETE_DATA_TYPE_CONFIRMATION_MESSAGE = "Please type \"{0}\"";

        /// <summary>
        /// text code to type to confirm delete data
        /// </summary>
        private const string DELETE_DATA_CONFIRMATION_CODE = "Delete Data";

        /// <summary>
        /// Stage data container
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stagesDefaultDataCollection;

        /// <summary>
        /// load data container
        /// </summary>
        [SerializeField] private SO_StageLoadData _stageDataLoad;

        /// <summary>
        /// delete data confirmation UI
        /// </summary>
        [SerializeField] private GameObject _deleteConfirmationUI;

        /// <summary>
        /// Type message confirmation text output
        /// </summary>
		[SerializeField] private TextMeshProUGUI _confirmationTypeMessageText;

        /// <summary>
        /// Input field confirmation to delete
        /// </summary>
		[SerializeField] private TMP_InputField _confirmationInputField;

        /// <summary>
        /// Button to confirm delete data
        /// </summary>
		[SerializeField] private Button _confirmButton;

        /// <summary>
        /// Button to close delete data confirmation UI
        /// </summary>
		[SerializeField] private Button _closeButton;


        private void Awake()
        {
            ApplicationEvents.Instance.OnPlayerPlanningToDeleteData = SetActiveConfirmationUI;

            _confirmationTypeMessageText.SetText(string.Format(DELETE_DATA_TYPE_CONFIRMATION_MESSAGE, DELETE_DATA_CONFIRMATION_CODE));

            _confirmButton.onClick.AddListener(ConfirmToDeleteData);
            _closeButton.onClick.AddListener(() => _deleteConfirmationUI.SetActive(false));
        }

        /// <summary>
        /// Show delete data confirmation UI
        /// </summary>
        private void SetActiveConfirmationUI()
        {
            _confirmationInputField.SetTextWithoutNotify(string.Empty);
            _deleteConfirmationUI.SetActive(true);
        }

        /// <summary>
        /// Confirm delete data action
        /// </summary>
        private void ConfirmToDeleteData()
        {
            if (!string.Equals(_confirmationInputField.text, DELETE_DATA_CONFIRMATION_CODE))
            {
                FloatingTextPool.Instance.ShowFloatingText(
                    INVALID_CONFIRMATION_TEXT_MESSAGE,
                    Input.mousePosition,
                    FloatingTextObj.Position_State.Screen,
                    FloatingTextObj.Text_State.Invalid);
                return;
            }

            ApplicationEvents.Instance.DeleteGameDataConfirmed(()=>
                StartCoroutine(ReloadDataAccount.ReloadGameplayData(
                    _stagesDefaultDataCollection,
                    _stageDataLoad,
                    false)
            ));
        }
    }
}
