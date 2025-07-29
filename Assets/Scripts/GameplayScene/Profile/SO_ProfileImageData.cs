namespace Project.Gameplay
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Profile Data/Profile Image Data")]
	public class SO_ProfileImageData : ScriptableObject
	{
		/// <summary>
		/// profile image id
		/// </summary>
		[SerializeField] private string _profileImageID;

		/// <summary>
		/// profile image id
		/// </summary>
		public string ProfileImageID => _profileImageID;

		/// <summary>
		/// profile image sprite
		/// </summary>
		[SerializeField] private Sprite _profileImageSprite;

		/// <summary>
		/// profile image sprite
		/// </summary>
		public Sprite ProfileImageSprite => _profileImageSprite;
	}
}
