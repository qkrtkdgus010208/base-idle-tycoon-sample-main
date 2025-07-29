namespace Project.Gameplay
{

    using UnityEngine;

	[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Stage Data/Order Table Data")]
	public class SO_StageOrderTablesData : ScriptableObject
	{
		/// <summary>
		/// Stage id for this order table stage elements data
		/// </summary>
		[SerializeField] private string _stageID;

		/// <summary>
		/// Stage id for this order table stage elements data
		/// </summary>
		public string StageID => _stageID;

		/// <summary>
		/// Order tables in this stage
		/// Specific Order table prefabs that will be instantiate in this stage
		/// prefabs bring instage elements (Size, position, rotation, etc)
		/// </summary>
		[SerializeField] private OrderTable[] _orderTables;

		/// <summary>
		/// Order tables in this stage
		/// Specific Order table prefabs that will be instantiate in this stage
		/// prefabs bring instage elements (Size, position, rotation, etc)
		/// </summary>
		public OrderTable[] OrderTable => _orderTables;
	}
}
