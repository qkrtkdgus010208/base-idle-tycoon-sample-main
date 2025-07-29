namespace Project.Gameplay
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Profile Data/Profile Image Data Collection")]
	public class SO_ProfileImageDataCollection : ScriptableObject
	{
		/// <summary>
		/// List of profile image data
		/// </summary>
		[SerializeField] private List<SO_ProfileImageData> _profileImageDataCollection;

		/// <summary>
		/// List of profile image data
		/// </summary>
		public ReadOnlyCollection<SO_ProfileImageData> ProfileImageDataCollection 
			=> _profileImageDataCollection.AsReadOnly();

		/// <summary>
		/// Get default profile image
		/// </summary>
		/// <returns> defualt profile image data </returns>
		public SO_ProfileImageData GetProfileImageDefaultData()
			=> _profileImageDataCollection[0];

		/// <summary>
		/// Get specific profile image data
		/// </summary>
		/// <param name="id"> target profile image id </param>
		/// <returns> profile image data </returns>
		public SO_ProfileImageData GetProfileImageDataByID(string id)
			=> _profileImageDataCollection.Find(x => string.Equals(x.ProfileImageID, id));
	}
}
