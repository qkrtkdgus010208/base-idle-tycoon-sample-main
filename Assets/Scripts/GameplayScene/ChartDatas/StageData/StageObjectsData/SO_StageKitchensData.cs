namespace Project.Gameplay
{

    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Stage Data/Kitchens Data")]
	public class SO_StageKitchensData : ScriptableObject
	{
		/// <summary>
		/// Stage ID for this kitchens stage elements data
		/// </summary>
		[SerializeField] private string _stageID;

		/// <summary>
		/// Stage ID for this kitchens stage elements data
		/// </summary>
		public string StageID => _stageID;

		/// <summary>
		/// Kitchenwares in this stage
		/// Specific kitchenware prefabs that will be instantiate in this stage
		/// prefabs bring instage elements (Size, position, rotation, etc)
		/// </summary>
		[SerializeField] private Kitchenware[] _kitchens;

		/// <summary>
		/// Kitchenwares in this stage
		/// Specific kitchenware prefabs that will be instantiate in this stage
		/// prefabs bring instage elements (Size, position, rotation, etc)
		/// </summary>
		public Kitchenware[] Kitchens => _kitchens;
	}
}
