namespace Project.Gameplay
{
    using UnityEngine;


    [CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Stage Data/Staff Data")]
    public class SO_StageStaffData : ScriptableObject
    {
        /// <summary>
        /// Stage ID for this staff stage elements data
        /// </summary>
        [SerializeField] private string _stageID;

        /// <summary>
        /// Stage ID for this staff stage elements data
        /// </summary>
        public string StageID => _stageID;

        /// <summary>
        /// Manager template for this stage
        /// Template elements (Size, Layer order, etc)
        /// </summary>
        [SerializeField] private StaffController _managerTemplate;

        /// <summary>
        /// Manager template for this stage
        /// Template elements (Size, Layer order, etc)
        /// </summary>
        public StaffController ManagerTemplate => _managerTemplate;

        /// <summary>
        /// Staff template for this stage
        /// Template elements (Size, Layer order, etc)
        /// </summary>
        [SerializeField] private StaffController _staffHelperTemplate;

        /// <summary>
        /// Staff template for this stage
        /// Template elements (Size, Layer order, etc)
        /// </summary>
        public StaffController StaffHelperTemplate => _staffHelperTemplate;

        /// <summary>
        /// Staff spawn points in this stage
        /// </summary>
        [SerializeField] private Transform[] _staffPoints;

        /// <summary>
        /// Staff spawn points in this stage
        /// </summary>
        public Transform[] StaffPoints => _staffPoints;
    }
}