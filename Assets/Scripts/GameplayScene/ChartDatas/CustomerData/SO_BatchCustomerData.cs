namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;


    [CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Customer Data/Batch Customer Data")]
    public class SO_BatchCustomerData : SO_ChartData
    {
        /// <summary>
        /// normal customer data
        /// </summary>
        [SerializeField] private CustomerData[] _normalCustomerData;

        /// <summary>
        /// List of normal customer object data
        /// </summary>
        [SerializeField] private List<SO_CustomerObjectData> _normalCustomer;

        /// <summary>
        /// List of normal customer object data
        /// </summary>
        public ReadOnlyCollection<SO_CustomerObjectData> NormalCustomer => _normalCustomer.AsReadOnly();

        /// <summary>
        /// List of special customer object data
        /// </summary>
        [SerializeField] private List<SO_CustomerObjectData> _specialCustomer;

        /// <summary>
        /// List of special customer object data
        /// </summary>
        public ReadOnlyCollection<SO_CustomerObjectData> SpecialCustomer => _specialCustomer.AsReadOnly();


        /// <summary>
		/// Initialize data from json data
		/// </summary>
		/// <param name="jsonData"> json to convert to data </param>
        public override void Initialize(string jsonData)
        {
            // Convert json into list of special customer data
            var specialCustomerData = Utility.StaticReflection.DatabaseItemsParse<CustomerData>(jsonData);

            foreach (var normalCustomer in _normalCustomerData) // assign customer data to customer object data for normal customer
                _normalCustomer.Find(x => x.CustomerID == normalCustomer.CustomerID).SetCustomerData(normalCustomer);

            foreach (var specialCustomer in specialCustomerData) // assign customer data to customer object data for special customer
                _specialCustomer.Find(x => x.CustomerID == specialCustomer.CustomerID).SetCustomerData(specialCustomer);
        }

        /// <summary>
        /// Get customer object data
        /// </summary>
        /// <param name="customerID"> Customer id </param>
        /// <returns> Customer object data </returns>
        public SO_CustomerObjectData GetCustomerObjectDataByID(CustomerData.Customer_ID customerID)
            => _normalCustomer.Find(x => x.CustomerID == customerID) ?? // looking for normal customer first, if found equals id return that customer data
            _specialCustomer.Find(x => x.CustomerID == customerID); // if customerID not found in normal customer, find customer id in special customer
    }
}
