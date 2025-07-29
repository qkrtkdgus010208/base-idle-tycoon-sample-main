namespace Project.Gameplay
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "Stall Environments", menuName = "Scriptable Objects/Stage Data/Stage Environments Data")]
	public class SO_StageEnvironmentsData : ScriptableObject
	{
		/// <summary>
		/// Stage ID for this environment stage elements data 
		/// </summary>
		[SerializeField] private string _stageID;

		/// <summary>
		/// Stage ID for this environment stage elements data 
		/// </summary>
		public string StageID => _stageID;

		/// <summary>
		/// Environtments object in this stage
		/// Prefabs that will be instantiate in this stage
		/// Prefabs bring instage elements (Size, position, rotation, etc)
		/// </summary>
		[SerializeField] private GameObject _environments;

		/// <summary>
		/// Prefabs that will be instantiate in this stage
		/// Prefabs bring instage elements (Size, position, rotation, etc)
		/// </summary>
		public GameObject Environments => _environments;
	}
}
