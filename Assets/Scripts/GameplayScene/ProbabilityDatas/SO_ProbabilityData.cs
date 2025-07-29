namespace Project.Gameplay
{
    using UnityEngine;

	public abstract class SO_ProbabilityData : ScriptableObject
	{
		/// <summary>
		/// Probability file id
		/// reference from Backnd console
		/// </summary>
		[SerializeField] private string _fileID;

		/// <summary>
		/// Probability file id
		/// reference from Backnd console
		/// </summary>
		public string FileID => _fileID;

		/// <summary>
		/// Initialize probability data
		/// </summary>
		public abstract void Initialize(string jsonData);
	}
}
