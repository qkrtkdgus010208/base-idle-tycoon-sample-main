namespace Project.Gameplay
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections.Generic;

    public class LeaderboardCoinsAccumulation : MonoBehaviour
	{
        /// <summary>
        /// Leaderboard locked message
        /// </summary>
        private const string LOCKED_MESSAGE = "Opens at Stage ";

        /// <summary>
        /// Leaderboad message to enter nickname
        /// </summary>
        private const string ENTER_NICKNAME_MESSAGE = "Enter your nickname first";

        /// <summary>
        /// You string to indicate user data
        /// </summary>
        private const string YOU_STR = "(you) ";

        /// <summary>
        /// leaderboard id reference to backnd console leaderboard
        /// </summary>
        [SerializeField] private string _leaderboardID;

        /// <summary>
        /// stage id that will unlocked leaderboard
        /// </summary>
        [SerializeField] private string _unlockedStageID;

        /// <summary>
        /// leaderboard data container
        /// </summary>
		[SerializeField] private SO_LeaderboardData _leaderboardData;

        /// <summary>
        /// stage load data container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _loaderData;

        /// <summary>
        /// stage data container
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stageDataCollection;

        /// <summary>
        /// Leaderboard UI
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// button to open leaderboard UI
        /// </summary>
        [SerializeField] private Button _openUIButton;

        /// <summary>
        /// Image open leaderboard ui button
        /// </summary>
        [SerializeField] private Image _openUIButtonImage;

        /// <summary>
        /// Locked image indicator
        /// </summary>
        [SerializeField] private GameObject _lockedIndicator;

        /// <summary>
        /// button to open leaderboard UI
        /// </summary>
        [SerializeField] private Button _closeUIButton;

        /// <summary>
        /// list of podium nickname text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI[] _podiumNicknameTexts;

        /// <summary>
        /// list of podium coins amount text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI[] _podiumCoinsTexts;

        /// <summary>
        /// Leaderboard list ui parent
        /// </summary>
        [SerializeField] private Transform _listParent;

        /// <summary>
        /// Leaderboar list ui template
        /// </summary>
        [SerializeField] private LeaderboardCoinsAccumulationListUI _listUITemplate;

        /// <summary>
        /// trophies icon
        /// </summary>
        [SerializeField] private Sprite[] _trophiesIcon;

        /// <summary>
        /// image for player trophy icon
        /// </summary>
        [SerializeField] private Image _playerTrophyIconImage;

        /// <summary>
        /// player rank text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _playerRankText;
        
        /// <summary>
        /// player nickname text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _playerNicknameText;

        /// <summary>
        /// player coin amount text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _playerCoinsText;

        /// <summary>
        /// leaderboard list ui pool
        /// </summary>
        private Utility.ClassPooling<LeaderboardCoinsAccumulationListUI> _leaderBoardUserListPool;
        
        /// <summary>
        /// Event to update user total coin in leaderboard
        /// </summary>
        public static Action<string, long> OnCoinsUpdated;

        /// <summary>
        /// accumulation coin leaderboard data from database
        /// </summary>
        private BackEnd.Leaderboard.LeaderboardTableItem _leaderboard;

        /// <summary>
        /// user data in accumulation coin leaderboard
        /// </summary>
        private BackEnd.Leaderboard.UserLeaderboardItem _myData;

        /// <summary>
        /// is leaderboard unlocked state
        /// true: when player reach stage with stage id _unlockedStageID, leadeboard unlocked
        /// false: leaderboard still locked
        /// </summary>
        private bool isLeaderboardUnlocked;


        private void Awake()
        {
            OnCoinsUpdated = (rowIndate, coinAccumulated) =>
            {
                if (string.IsNullOrEmpty(BackndServer.BackndUserInfo.Instance.UserInfo.nickname))
                    return;

                if (_leaderboard == null)
                    _leaderboard = _leaderboardData.LeaderboardTableList.Find(x => string.Equals(x.uuid, _leaderboardID));

                _leaderboardData.UpdateLeaderboardDataAsync(
                    _leaderboardID,
                    _leaderboard.table,
                    rowIndate,
                    _leaderboard.column,
                    coinAccumulated
                );
            };

            _openUIButton.onClick.AddListener(() => SetActiveUI(true));
            _closeUIButton.onClick.AddListener(() => SetActiveUI(false));

            isLeaderboardUnlocked = 
                _stageDataCollection.GetStageIdxById(_loaderData.CurrentStageID) >= 
                _stageDataCollection.GetStageIdxById(_unlockedStageID);

            _openUIButtonImage.color = isLeaderboardUnlocked ? _openUIButton.colors.normalColor : _openUIButton.colors.disabledColor;
            _lockedIndicator.SetActive(!isLeaderboardUnlocked);

            _leaderBoardUserListPool = new Utility.ClassPooling<LeaderboardCoinsAccumulationListUI>(
                () => Instantiate(_listUITemplate, _listParent));

            _leaderboardData.LoadData();
        }

        /// <summary>
        /// Initialize coin accumulation leaderboard data
        /// </summary>
        private void Initialize()
        {
            _leaderBoardUserListPool.ClearActiveObj();

            _leaderboardData.GetMyDataInLeaderboard(_leaderboardID,
                (myData) =>
                {
                    _myData = myData;

                    int myRankIdx = int.Parse(myData.rank) - 1;
                    _playerTrophyIconImage.sprite = _trophiesIcon[Mathf.Min(myRankIdx, _trophiesIcon.Length - 1)];
                    _playerRankText.SetText(myRankIdx < _trophiesIcon.Length - 1 ? string.Empty : myData.rank);
                    _playerNicknameText.SetText(YOU_STR + myData.nickname);
                    _playerCoinsText.SetText(Utility.StaticCurrencyStringConverison.GetString(long.Parse(myData.score)));
                });

            _leaderboardData.GetLeaderboardByID(_leaderboardID,
                (leaderboardData) =>
                {
                    foreach (var data in leaderboardData)
                    {
                        int rank = int.Parse(data.rank);

                        int podiumIdx = rank - 1;
                        if (podiumIdx < _podiumNicknameTexts.Length)
                        {
                            _podiumNicknameTexts[podiumIdx].SetText(data.nickname);
                            _podiumCoinsTexts[podiumIdx].SetText(Utility.StaticCurrencyStringConverison.GetString(long.Parse(data.score)));
                        }

                        var listUI = _leaderBoardUserListPool.GetFromPool();
                        listUI.Initialize(rank, data.nickname, long.Parse(data.score));
                        listUI.SetActive(true);
                    }
                });
        }

        /// <summary>
        /// Set active leaderboard UI
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveUI(bool state)
        {
            if (state)
            {
                if (!isLeaderboardUnlocked)
                {
                    FloatingTextPool.Instance.ShowFloatingText(
                        LOCKED_MESSAGE + (_stageDataCollection.GetStageIdxById(_unlockedStageID) + 1),
                        Input.mousePosition,
                        FloatingTextObj.Position_State.Screen,
                        FloatingTextObj.Text_State.Normal
                    );

                    return;
                }

                if (string.IsNullOrEmpty(BackndServer.BackndUserInfo.Instance.UserInfo.nickname))
                {
                    ProfileManager.OpenProfileUI?.Invoke();

                    FloatingTextPool.Instance.ShowFloatingText(
                        ENTER_NICKNAME_MESSAGE,
                        Input.mousePosition,
                        FloatingTextObj.Position_State.Screen,
                        FloatingTextObj.Text_State.Normal
                    );

                    return;
                }

                if (_myData == null)
                {
                    if (_leaderboard == null)
                        _leaderboard = _leaderboardData.LeaderboardTableList.Find(x => string.Equals(x.uuid, _leaderboardID));

                    _leaderboardData.UpdateLeaderboardData(
                        _leaderboardID,
                        _leaderboard.table,
                        SaveData.UserDataManager.Instance.TableRowIndate,
                        _leaderboard.column,
                        SaveData.UserDataManager.Instance.TotalCoinEarned
                    );

                    Initialize();
                }
            }

            _ui.SetActive(state);
        }
    }
}
