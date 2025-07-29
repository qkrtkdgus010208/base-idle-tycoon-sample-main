namespace Project.Gameplay
{
    using UnityEngine;


    [CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Stage Data/Customer Data")]
    public class SO_StageCustomerData : ScriptableObject
    {
        /// <summary>
        /// Stage ID for this customer stage elements data
        /// </summary>
        [SerializeField] private string _stageID;

        /// <summary>
        /// Stage ID for this customer stage elements data
        /// </summary>
        public string StageID => _stageID;

        /// <summary>
        /// Customer object template for this stage
        /// Template elements (Size, Layer order, etc)
        /// </summary>
        [SerializeField] private CustomerController _customerTemplate;

        /// <summary>
        /// Customer object template for this stage
        /// Template elements (Size, Layer order, etc)
        /// </summary>
        public CustomerController CustomerTemplate => _customerTemplate;

        /// <summary>
        /// Customer spawn points in this stage
        /// </summary>
        [SerializeField] private Transform[] _customerSpawnPoint;

        /// <summary>
        /// Customer spawn points in this stage
        /// </summary>
        public Transform[] CustomerSpawnPoint => _customerSpawnPoint;

        /// <summary>
        /// Customer despawn points in this stage
        /// </summary>
        [SerializeField] private Transform[] _customerDespawnPoint;

        /// <summary>
        /// Customer despawn points in this stage
        /// </summary>
        public Transform[] CustomerDespawnPoint => _customerDespawnPoint;
    }
}