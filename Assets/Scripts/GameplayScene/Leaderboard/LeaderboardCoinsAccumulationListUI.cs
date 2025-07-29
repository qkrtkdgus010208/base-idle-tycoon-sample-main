namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


	public class LeaderboardCoinsAccumulationListUI : MonoBehaviour, Utility.IPoolingObjects
	{
		/// <summary>
		/// Leaderboard list ui elements data
		/// </summary>
		[SerializeField] private LeadeboardUIListElement[] _elementsData;

		/// <summary>
		/// Image for base frame
		/// </summary>
		[SerializeField] private Image _basePanel;

		/// <summary>
		/// Image for trophy icon
		/// </summary>
		[SerializeField] private Image _trophyIcon;

		/// <summary>
		/// ranking text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _rankingTextOutput;
		
		/// <summary>
		/// nickname text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _nicknameTextOutput;

		/// <summary>
		/// coin amount text output
		/// </summary>
		[SerializeField] private TextMeshProUGUI _coinTextOutput;

		/// <summary>
		/// Object pool activate state
		/// </summary>
		private bool isActive;

		/// <summary>
		/// Object pool activate state
		/// </summary>
		/// <returns> true: object active / false: object inactive </returns>
		public bool IsActive() => isActive;

		/// <summary>
		/// Set active object pool
		/// </summary>
		/// <param name="state"> true: set active / false: deactive </param>
		public void SetActive(bool state)
		{
			isActive = state;
			gameObject.SetActive(state);
		}

		/// <summary>
		/// Initialize list ui
		/// </summary>
		/// <param name="rank"> rank number </param>
		/// <param name="nickname"> nickname </param>
		/// <param name="score"> score amount </param>
		public void Initialize(int rank, string nickname, long score)
        {
			var elmentIdx = _elementsData[Mathf.Min(rank, _elementsData.Length) - 1];

			_basePanel.sprite = elmentIdx.BasePanel;
			_trophyIcon.sprite = elmentIdx.TrophyIcon;
			_nicknameTextOutput.color = elmentIdx.TextColor;
			_coinTextOutput.color = elmentIdx.TextColor;

			_rankingTextOutput.SetText(rank < _elementsData.Length - 1? string.Empty : rank.ToString());
			_nicknameTextOutput.SetText(nickname);
			_coinTextOutput.SetText(Utility.StaticCurrencyStringConverison.GetString(score));
		}
        

        [System.Serializable]
		public struct LeadeboardUIListElement
        {
			public Sprite BasePanel;
			public Sprite TrophyIcon;
			public Color TextColor;
        }
	}
}
