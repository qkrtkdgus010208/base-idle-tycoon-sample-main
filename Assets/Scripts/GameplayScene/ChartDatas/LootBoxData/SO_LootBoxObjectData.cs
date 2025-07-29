namespace Project.Gameplay
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Loot Box/Loot Box Object Data")]
	public class SO_LootBoxObjectData : ScriptableObject
	{
		/// <summary>
		/// Lootbox ID
		/// </summary>
		[SerializeField] private string _lootBoxID;

		/// <summary>
		/// Lootbox ID
		/// </summary>
		public string LootBoxID => _lootBoxID;

		/// <summary>
		/// Lootbox sprite when closed
		/// </summary>
		[SerializeField] private Sprite _closeSpriteObject;

		/// <summary>
		/// Lootbox sprite when closed
		/// </summary>
		public Sprite CloseSpriteObject => _closeSpriteObject;

		/// <summary>
		/// Lootbox sprite when opened
		/// </summary>
		[SerializeField] private Sprite _openSpriteObject;

		/// <summary>
		/// Lootbox sprite when opened
		/// </summary>
		public Sprite OpenSpriteObject => _openSpriteObject;

		/// <summary>
		/// lootbox data from database
		/// </summary>
		private LootBoxData _lootBoxData;

		/// <summary>
		/// lootbox data from database
		/// </summary>
		public LootBoxData LootboxData => _lootBoxData;

		/// <summary>
		/// Set lootbox data for this lootbox object
		/// </summary>
		/// <param name="lootBoxData"> lootbox data from database </param>
		public void SetLootboxData(LootBoxData lootBoxData) 
			=> _lootBoxData = lootBoxData;
	}
}
