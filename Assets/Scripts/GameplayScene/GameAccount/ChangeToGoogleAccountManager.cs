namespace Project.Gameplay
{
    using BackndServer;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using TMPro;
    using Project.Gameplay.SaveData;

    public class ChangeToGoogleAccountManager : MonoBehaviour
	{
        /// <summary>
        /// Google login success message
        /// </summary>
        private const string GOOGLE_LOGIN_SUCCESS_MSG = "Google Login Success";

        /// <summary>
        /// Conflict data account onplayed stage information message
        /// </summary>
        private const string CONFLICT_DATA_ACCOUNT_ONPLAYED_STAGE_MESSAGE = "Your stage progression in this account:\n";

        /// <summary>
        /// Overwrite confirmation message
        /// </summary>
        private const string OVERWRITE_MESSAGE = "Your google account data will be overwrite to current progression";

        /// <summary>
        /// Leave current progression confirmation message
        /// </summary>
        private const string LEAVE_CURRENT_PROGRESSION_MESSAGE = "Your current progression data will be lost";

        /// <summary>
        /// Stage default data
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stagesDefaultDataCollection;

        /// <summary>
        /// SO stage load object data
        /// </summary>
        [SerializeField] private SO_StageLoadData _stageDataLoad;

        [Header("Account conflict data action UI")]

        /// <summary>
        /// Conflict data confirmation ui
        /// Active when player change login to federation account that already have data
        /// </summary>
        [SerializeField] private GameObject _conflictDataConfirmationUI;

        /// <summary>
        /// conflict account data onplayed stage info text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _conflictDataAccountOnPlayedStageNameText;

        /// <summary>
        /// Button to overwrite data in federation account to current progression data
        /// </summary>
        [SerializeField] private Button _keepCurrentProgressionButton;

        /// <summary>
        /// Button to leave current progression data then proceed to federation account login
        /// </summary>
        [SerializeField] private Button _keepGoogleAccountButton;

        /// <summary>
        /// Cancel button
        /// </summary>
        [SerializeField] private Button _cancelButton;
        


        [Space(10)]
        [Header("Additional conflict data confirmation UI")]

        /// <summary>
        /// Additional conflict confirmation ui
        /// Active when player decided to leave / overwrite account
        /// </summary>
        [SerializeField] private GameObject _additionalConflictDataConfirmationUI;

        /// <summary>
        /// Additional confirmation message text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _additionalConflictDataConfiramtionMessage;

        /// <summary>
        /// Confirm button to leave / overwrite account
        /// </summary>
        [SerializeField] private Button _additionalConflictDataConfirmationConfirmButton;

        /// <summary>
        /// Cancel button to leave / overwrite account
        /// </summary>
        [SerializeField] private Button _additionalConflictDataConfirmationCancelButton;

        /// <summary>
        /// federation account token
        /// </summary>
        private string currentFederationAccountToken;

        /// <summary>
        /// is override state
        /// </summary>
        private bool isOverride;

        private void Awake()
        {
            ApplicationEvents.Instance.OnChangeToGoogleLogin = ChangeToGoogleLogin; // Asign OnChangeToGoogleLogin Events

            _keepCurrentProgressionButton.onClick.AddListener(KeepCurrentProgressionConfirmation);
            _keepGoogleAccountButton.onClick.AddListener(KeepGoogleAccountConfirmation);
            _cancelButton.onClick.AddListener(CancelChangeToGoogleLogin);

            _additionalConflictDataConfirmationConfirmButton.onClick.AddListener(ProceedGoogleLogin);
            _additionalConflictDataConfirmationCancelButton.onClick.AddListener(
                () => _additionalConflictDataConfirmationUI.SetActive(false));
        }

        /// <summary>
        /// for player that login by guest login
        /// and want to change to google login
        /// </summary>
        private void ChangeToGoogleLogin()
        {
            LoadingUIController.Instance.Loading(); // Start Loading

            BackndLogin.ChangeCustomToFederationLogin(
                ChangeToGoogleLoginResultCallback, // Change guest login to federation login result
                ChangeToGoogleLoginConflictDataConfirmation); // Conflict data action confirmation (if federation target account already have game data)

            LoadingUIController.Instance.FinishLoading(); // End Loading
        }

        /// <summary>
        /// On Change Custom To Federation Login Result Callback
        /// </summary>
        /// <param name="isSuccess"> change to federation login success state </param>
        private void ChangeToGoogleLoginResultCallback(bool isSuccess)
        {
            if (isSuccess) // show flash notice when federation login success
            {
                SettingManager.SetGoogleLoginButtonMute?.Invoke();
                NoticeUIController.Instance.ShowFlashNotice(GOOGLE_LOGIN_SUCCESS_MSG);
            }
        }

        /// <summary>
        /// On Change Custom To Federation Login 
        /// but there is conflict data need action confirmation
        /// </summary>
        private void ChangeToGoogleLoginConflictDataConfirmation(string federationToken)
        {
            currentFederationAccountToken = federationToken;

            var stageData = UserStageDataManager.Instance.GetPlayerUserStageTemporaryLoginData();

            // Look up for highest stage level
            string highestStageName = _stagesDefaultDataCollection.StageDataCollection[0].StageName; // Assign to first stage (handler if player have no stage data yet)
            for (int i = _stagesDefaultDataCollection.StageDataCollection.Count - 1; i > 0; i--) // Look up for highest stage level first
            {
                var highestStage = stageData.Find(x => string.Equals(x.StageID, _stagesDefaultDataCollection.StageDataCollection[i].StageID));
                if (highestStage.StageID == _stagesDefaultDataCollection.StageDataCollection[i].StageID) // If player has saved stage id
                {
                    highestStageName = _stagesDefaultDataCollection.StageDataCollection[i].StageName; // Set stageToLoad to highest stage level stage id found in UserStageData
                    break;
                }
            }

            _conflictDataAccountOnPlayedStageNameText.SetText(
                CONFLICT_DATA_ACCOUNT_ONPLAYED_STAGE_MESSAGE + highestStageName);

            _conflictDataConfirmationUI.SetActive(true);
        }

        /// <summary>
        /// When found data in google account
        /// And player cancel login to that account 
        /// perhaps want to change to another account
        /// </summary>
        private void CancelChangeToGoogleLogin()
        {
            LogoutGoogleAccount(null);
            _conflictDataConfirmationUI.SetActive(false);
        }

        /// <summary>
        /// Activate confirmation before overwrite google account data
        /// </summary>
        private void KeepCurrentProgressionConfirmation()
        {
            isOverride = true;
            _additionalConflictDataConfiramtionMessage.SetText(OVERWRITE_MESSAGE);
            _additionalConflictDataConfirmationUI.SetActive(true);
        }

        /// <summary>
        /// Activate confirmation before leaving current progression
        /// </summary>
        private void KeepGoogleAccountConfirmation()
        {
            isOverride = false;

            _additionalConflictDataConfiramtionMessage.SetText(LEAVE_CURRENT_PROGRESSION_MESSAGE);
            _additionalConflictDataConfirmationUI.SetActive(true);
        }

        /// <summary>
        /// Leave current progression data then proceed to federation account login
        /// </summary>
        private void ProceedGoogleLogin()
        {
            LoadingUIController.Instance.Loading(); // Start Loading

            if (!LogoutBackndAccount())
            {
                LoadingUIController.Instance.FinishLoading(); // Finish Loading
                return;
            }

            BackndLogin.LoginGoogleWithFederationToken(currentFederationAccountToken);
            
            LoadingUIController.Instance.FinishLoading(); // Finish Loading

            StartCoroutine(ReloadDataAccount.ReloadGameplayData(
                _stagesDefaultDataCollection,
                _stageDataLoad,
                isOverride));
        }

        /// <summary>
        /// Logout google account
        /// </summary>
        /// <param name="resultCallbacks"> Result Callback </param>
        private void LogoutGoogleAccount(Action<bool> resultCallbacks)
        {
            BackndLogin.GoogleSignOut(resultCallbacks);
        }

        /// <summary>
        /// Logout backnd account
        /// </summary>
        private bool LogoutBackndAccount()
        {
            bool result = BackndLogin.LogoutAccount();
            if (!result)
                Debug.Log("Logout Failed");

            return result;
        }
    }
}
