namespace Project.Gameplay
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Dress/Dress Object Data")]
	public class SO_DressObjectData : ScriptableObject
	{
		/// <summary>
		/// Dress id
		/// </summary>
		[SerializeField] private string _dressID;

		/// <summary>
		/// Dress id
		/// </summary>
		public string DressID => _dressID;

		/// <summary>
		/// Dress icon
		/// </summary>
		[SerializeField] private Sprite _icon;

		/// <summary>
		/// Dress icon
		/// </summary>
		public Sprite Icon => _icon;

		/// <summary>
		/// Dress object sprite
		/// </summary>
		[SerializeField] private Sprite _spriteObject;

		/// <summary>
		/// Dress object sprite
		/// </summary>
		public Sprite SpriteObject => _spriteObject;

		/// <summary>
		/// Dress skin animator
		/// </summary>
		[SerializeField] private RuntimeAnimatorController _animator;

		/// <summary>
		/// Dress skin animator
		/// </summary>
		public RuntimeAnimatorController Animator => _animator;
	}
}
