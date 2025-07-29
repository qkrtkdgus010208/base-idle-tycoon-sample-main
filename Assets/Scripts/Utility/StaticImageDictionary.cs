namespace Project.Utility
{
    using System.Collections.Generic;
    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Static Image Dictionary")]
	public class StaticImageDictionary : ScriptableObject
	{
		/// <summary>
		/// Singleton
		/// </summary>
		private static StaticImageDictionary _instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static StaticImageDictionary Instance => _instance;


		/// <summary>
		/// Image data storage
		/// </summary>
		[SerializeField] private List<ImageData> _imageDatas;



		/// <summary>
		/// Initialize Singleton
		/// </summary>
		public void InitializeSingleton()
        {
			if (Instance != null) // Checking if singleton is already assigned
				return;

			_instance = this; // Assign singleton
        }

		/// <summary>
		/// Get sprite asset by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Sprite GetImageSpriteByID(string id)
			=> _imageDatas.Find(x => string.Equals(x.ID, id)).Image; // Look up for ImageData that have ID equals with id

		[System.Serializable]
		public struct ImageData
        {
			public string ID; // Image ID
			public Sprite Image; // Image Sprite
        }
	}
}
