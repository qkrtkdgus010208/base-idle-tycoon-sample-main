namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


    public class CustomerSpawner : MonoBehaviour
    {
        /// <summary>
        /// Randomize rate to apply customer that spawn normal / special customer
        /// </summary>
        private const int RANDOMIZE_RATE = 100;

        /// <summary>
        /// Special customer spawn chance per RANDOMIZE_RATE
        /// 25 means chance to special customer spawn instead the normal one is 25 / 100 (RANDOMIZE_RATE)
        /// </summary>
        private const int SPECIAL_CUSTOMER_SPAWN_CHANCE = 25;

        /// <summary>
        /// Data loader container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _stageLoadData;
        
        /// <summary>
        /// customer data container
        /// </summary>
        [SerializeField] private SO_BatchCustomerData _customerDataCollection;

        /// <summary>
        /// stage data container
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stageDefaultDataCollection;

        /// <summary>
        /// customer spawn points
        /// </summary>
        private Transform[] _spawnPoint;

        /// <summary>
        /// customer despawn points
        /// </summary>
        private Transform[] _despawnPoint;

        /// <summary>
        /// customer controller template
        /// </summary>
        private CustomerController _customerTemplate;

        /// <summary>
        /// customer object pooling
        /// </summary>
        private Dictionary<CustomerData.Customer_ID, List<CustomerController>> customerPool;

        /// <summary>
        /// customer spawned total
        /// to given new customer spawned customer idx
        /// </summary>
        private int customerTotal;

        /// <summary>
        /// customer spawn time rate
        /// based on each stage customer spawn time rate
        /// </summary>
        private float _customerSpawnTime;

        /// <summary>
        /// customer spawn timer counter
        /// </summary>
        private float spawnTimer;


        private void Start()
        {
            LoadingUIController.Instance.Loading();

            var loadData = _stageLoadData.CurrentStageCustomerData;

            _spawnPoint = new Transform[loadData.CustomerSpawnPoint.Length];
            for (int i = 0; i < _spawnPoint.Length; i++)
                _spawnPoint[i] = Instantiate(loadData.CustomerSpawnPoint[i]);

            _despawnPoint = new Transform[loadData.CustomerDespawnPoint.Length];
            for (int i = 0; i < _despawnPoint.Length; i++)
                _despawnPoint[i] = Instantiate(loadData.CustomerDespawnPoint[i]);

            _customerTemplate = loadData.CustomerTemplate;

            _customerSpawnTime = _stageDefaultDataCollection.GetStageDataById(_stageLoadData.CurrentStageID).CustomerSpawnTime;

            customerPool = new Dictionary<CustomerData.Customer_ID, List<CustomerController>>();

            LoadingUIController.Instance.FinishLoading();
        }

        private void Update()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= _customerSpawnTime)
            {
                spawnTimer -= _customerSpawnTime;
                SpawnCustomer();
            }
        }

        /// <summary>
        /// Instantiate new customer if pool need more
        /// </summary>
        /// <param name="customerID"> customer variant id </param>
        /// <returns> new customer ready to spawn </returns>
        private CustomerController InstantiateCustomer(CustomerData.Customer_ID customerID)
        {
            CustomerController customer = Instantiate(_customerTemplate, _spawnPoint[Random.Range(0, _spawnPoint.Length)].position, Quaternion.identity);

            customer.SetCustomerElement(_customerDataCollection.GetCustomerObjectDataByID(customerID));

            if (!customerPool.ContainsKey(customerID))
                customerPool.Add(customerID, new List<CustomerController>());

            customerPool[customerID].Add(customer);
            return customer;
        }

        /// <summary>
        /// Spawn customer
        /// </summary>
        private void SpawnCustomer()
        {
            if (!StageEventsManager.IsAnyAvailableOrderTable()) return;

            customerTotal++;

            CustomerController customer = GetCustomerToSpawn();
            customer.Spawn(
                customerTotal,
                _spawnPoint[Random.Range(0, _spawnPoint.Length)].position,
                _despawnPoint[Random.Range(0, _despawnPoint.Length)].position);
        }

        /// <summary>
        /// Get customer from pool to spawn
        /// </summary>
        /// <returns> customer from pool ready to spawn </returns>
        private CustomerController GetCustomerToSpawn()
        {
            var randID = RandomizeCustomerVariant();

            if (customerPool.ContainsKey(randID))
            {
                foreach (CustomerController customer in customerPool[randID])
                    if (!customer.gameObject.activeInHierarchy)
                        return customer;
            }

            return InstantiateCustomer(randID);
        }

        /// <summary>
        /// Randomize customer variant that will spawn
        /// </summary>
        /// <returns> customer id variant that need to spawn </returns>
        private CustomerData.Customer_ID RandomizeCustomerVariant()
        {
            return Random.Range(0, RANDOMIZE_RATE) < SPECIAL_CUSTOMER_SPAWN_CHANCE ?
                   _customerDataCollection.SpecialCustomer[Random.Range(0, _customerDataCollection.SpecialCustomer.Count)].CustomerID :
                   _customerDataCollection.NormalCustomer[Random.Range(0, _customerDataCollection.NormalCustomer.Count)].CustomerID;
        }
    }
}