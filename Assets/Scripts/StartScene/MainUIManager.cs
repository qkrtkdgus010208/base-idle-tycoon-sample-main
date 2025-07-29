namespace Project.StartScene
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections;

    public class MainUIManager : MonoBehaviour
    {
        /// <summary>
        /// UI game icon & Backnd logo
        /// </summary>
        [SerializeField] private GameObject _splashScreenUI;

        /// <summary>
        /// UI background
        /// </summary>
        [SerializeField] private GameObject _backgroundUI;

        /// <summary>
        /// UI tap to start text
        /// </summary>
        [SerializeField] private GameObject _tapToEnterGameText;

        /// <summary>
        /// full screen button for tap to start
        /// </summary>
        [SerializeField] private Button _tapToEnterGameButton;

        /// <summary>
        /// splash screen active time
        /// </summary>
        private YieldInstruction _awaitSplashScreenTime = new WaitForSeconds(4f);



        [Space(10)]
        [Header("Login UI")]
        /// <summary>
        /// Login UI Buttons panel, active if player login with token failed
        /// </summary>
        [SerializeField] private GameObject _loginUI;

        /// <summary>
        /// player prefs data (debug mode only)
        /// </summary>
        [SerializeField] private Button _localDataButton;

        /// <summary>
        /// sign up, generate account as custom login (debug mode only)
        /// </summary>
        [SerializeField] private Button _signUpButton;

        /// <summary>
        /// login or generate account as guest
        /// </summary>
        [SerializeField] private Button _guestButton;

        /// <summary>
        /// google federation login
        /// </summary>
        [SerializeField] private Button _googleLoginButton;


        [Space(10)]
        [Header("Sign up UI")]
        /// <summary>
        /// Sign up UI (debug mode only)
        /// </summary>
        [SerializeField] private GameObject _signUpUI;

        /// <summary>
        /// ID input UI (debug mode only)
        /// </summary>
        [SerializeField] private TMP_InputField _IDinput;

        /// <summary>
        /// password input UI (debug mode only)
        /// </summary>
        [SerializeField] private TMP_InputField _PWinput;

        /// <summary>
        /// Confirm account (debug mode only)
        /// </summary>
        [SerializeField] private Button _confirmButton;

        /// <summary>
        /// Sign input value player id
        /// </summary>
        private string _id;

        /// <summary>
        /// Sign input value player password
        /// </summary>
        private string _pw;

        private void Awake()
        {
            StaticStartSceneEventsManager.SetActiveLoginUI = SetActiveLoginUI; // Assign set active login UI
            StaticStartSceneEventsManager.SetActiveEnterTheGameUI = SetActiveTapToStart; // Assign set active enter the game UI

            _tapToEnterGameButton.onClick.AddListener(() => StaticStartSceneEventsManager.OnEnterTheGame?.Invoke()); // Push OnEnterTheGame events

            _guestButton.onClick.AddListener(BackndServer.BackndLogin.GuestLogin); // Guest login
            _googleLoginButton.onClick.AddListener(BackndServer.BackndLogin.GoogleLogin); // Federation google login

            _localDataButton.onClick.AddListener(() => DataLoader.IsLocalData = true); // Set to local data

            _signUpButton.onClick.AddListener(() => // Sign up button events
            {
                _loginUI.SetActive(false); // Close login UI
                _signUpUI.SetActive(true); // Set active sign up UI
            });

            _IDinput.onValueChanged.AddListener(SetID); // Input ID events
            _PWinput.onValueChanged.AddListener(SetPW); // Input Password events

            _confirmButton.onClick.AddListener(() => // Confirm sign up button events
            {
                if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_pw)) // if ID or password not set yet
                    return;

                BackndServer.BackndLogin.CustomSignUp(_id, _pw); // Create account on backnd console

                _loginUI.SetActive(true); // Set active login UI
                _signUpUI.SetActive(false); // Close sign up UI
            });

            StartCoroutine(SplashScreenShowingUp()); // Start splash screen
        }

        /// <summary>
        /// Set active tap to start UI
        /// </summary>
        private void SetActiveTapToStart() 
        {
            _backgroundUI.SetActive(true); // Set active UI
            _tapToEnterGameButton.gameObject.SetActive(true); // Set active tap to start UI
        }

        /// <summary>
        /// Set active login UI
        /// </summary>
        private void SetActiveLoginUI()
        {
            #if UNITY_EDITOR
                _googleLoginButton.interactable = false; // Google login is unavailable in unity editor
            #endif

            _backgroundUI.SetActive(true); // Set active UI
            _loginUI.SetActive(true); // Set active login buttons panel
        }

        /// <summary>
        /// Set player ID
        /// </summary>
        /// <param name="id"></param>
        private void SetID(string id) => _id = id;

        /// <summary>
        /// Set player password
        /// </summary>
        /// <param name="pw"></param>
        private void SetPW(string pw) => _pw = pw;

        /// <summary>
        /// Set active splash screen
        /// </summary>
        /// <returns></returns>
        private IEnumerator SplashScreenShowingUp()
        {
            _splashScreenUI.SetActive(true); // show splash screen
            yield return _awaitSplashScreenTime; // splash screen active time
            _splashScreenUI.SetActive(false); // hide splash screen

            StaticStartSceneEventsManager.OnSplashScreenDone?.Invoke(); // Push OnSplashScreenDone events
        }
    }
}
