namespace Project.Gameplay
{
	/// <summary>
	/// Dress data
	/// </summary>
	[System.Serializable]
	public struct DressData
	{
		/// <summary>
		/// Dress ID (back end side)
		/// </summary>
		public string DressID;

		/// <summary>
		/// Dress Name output name
		/// </summary>
		public string ItemName;

		/// <summary>
		/// Dress rarity rank
		/// </summary>
		public string Rarity;

		/// <summary>
		/// Dress buff efect code from data base
		/// </summary>
		public string EffectCode;

		/// <summary>
		/// Dress effect description
		/// </summary>
		public string EffectDescription;
	}

	/// <summary>
	/// Dress object
	/// Transformation dress data into object
	/// </summary>
	[System.Serializable]
	public class DressObject
    {
		/// <summary>
		/// Buff code data separator
		/// </summary>
		private const char BUFF_DATA_SEPARATOR = '_';

		/// <summary>
		/// Dress data
		/// </summary>
		private DressData _dressData;

		/// <summary>
		/// Dress object data
		/// </summary>
		private SO_DressObjectData _dressObjectData;

		/// <summary>
		/// Buff efect object
		/// </summary>
		private AbstractDressBuffEffects _buffEffect;

		/// <summary>
		/// Dress data ID
		/// </summary>
		public string DressID => _dressData.DressID;

		/// <summary>
		/// Dress data name
		/// </summary>
		public string DressName => _dressData.ItemName;

		/// <summary>
		/// Dress data rarity
		/// </summary>
		public string Rarity => _dressData.Rarity;

		/// <summary>
		/// Dress buff effect description
		/// </summary>
		public string EffectBonusDescription => _dressData.EffectDescription;

		/// <summary>
		/// Checking if dress have effect
		/// </summary>
		public bool IsHaveEffect => !string.IsNullOrEmpty(_dressData.EffectCode);

		/// <summary>
		/// Dress sprite
		/// </summary>
		public UnityEngine.Sprite SpriteObject => _dressObjectData.SpriteObject;

		public DressObject(DressData dressData, SO_DressObjectData dressObjectData)
		{
			_dressData = dressData;
			_dressObjectData = dressObjectData;

			var buffStr = _dressData.EffectCode.Split(BUFF_DATA_SEPARATOR); // Parse buff effect code into buff object
			_buffEffect = buffStr[0] switch
			{
				BonusProfit.BUFF_STR_KEY => new BonusProfit(int.Parse(buffStr[1])),
				BonusMovement.BUFF_STR_KEY => new BonusMovement(int.Parse(buffStr[1])),
				_ => null
			};
		}

		/// <summary>
		/// Wear this dress
		/// </summary>
		/// <param name="characterAnim"> character animator controller </param>
		public void WearDress(UnityEngine.Animator characterAnim)
		{
			characterAnim.runtimeAnimatorController = _dressObjectData.Animator;

			if (IsHaveEffect)
				_buffEffect.SetActiveEffect(true);
		}

		/// <summary>
		/// Remove dress
		/// </summary>
		public void RemoveDress()
		{
			if (IsHaveEffect)
				_buffEffect.SetActiveEffect(false);
		}
	}
}
