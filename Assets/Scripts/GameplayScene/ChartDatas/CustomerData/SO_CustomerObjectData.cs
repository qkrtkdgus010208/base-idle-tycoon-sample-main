namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.Animations;


    [CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Customer Data/Customer Object Data")]
	public class SO_CustomerObjectData : ScriptableObject
	{
        /// <summary>
        /// Customer effect code separator between effect id and effect bonus value
        /// </summary>
        private const char STR_EFFECT_CODE_KEY_SEPARATOR = '_';

        /// <summary>
        /// Customer id reference to item in chart customer data
        /// </summary>
        [SerializeField] private CustomerData.Customer_ID _customerID;

        /// <summary>
        /// Customer id reference to item in chart customer data
        /// </summary>
        public CustomerData.Customer_ID CustomerID => _customerID;

        /// <summary>
        /// Customer skin
        /// </summary>
        [SerializeField] private RuntimeAnimatorController _customerAsset;

        /// <summary>
        /// Customer skin
        /// </summary>
        public RuntimeAnimatorController CustomerAsset => _customerAsset;
        
        /// <summary>
        /// Customer name
        /// </summary>
        private string _customerName;

        /// <summary>
        /// Customer name
        /// </summary>
        public string CustomerName => _customerName;

        /// <summary>
        /// Customer effect object
        /// </summary>
        private AbstractCustomerEffect _customerEffect;

        /// <summary>
        /// Customer effect object
        /// </summary>
        public AbstractCustomerEffect CustomerEffect => _customerEffect;

        /// <summary>
        /// Assign customer data to this customer object
        /// </summary>
        /// <param name="customerData"> customer data </param>
        public void SetCustomerData(CustomerData customerData)
        {
            _customerName = customerData.CustomerName; // Assign customer data
            _customerEffect = string.IsNullOrEmpty(customerData.EffectData) ? // checking is customer data have effect
                new CustomerEffectNone() : // if there is no effect data, assign none custmer effect
                ParseCustomerEffectData(customerData.EffectData.Split(STR_EFFECT_CODE_KEY_SEPARATOR)); // if customer have effect data, assign customer effect based on effect data code
        }

        /// <summary>
        /// Parsing string customer effect data code to customer effect
        /// </summary>
        /// <param name="effectData"> string effect code </param>
        /// <returns> customer effect object </returns>
        private AbstractCustomerEffect ParseCustomerEffectData(string[] effectData)
        {
            // example effect code : "BP_1.5";
            // separate by '_'
            // effectdata first idx is effect code
            // effectdata second idx is bonus value

            return effectData[0] switch
            {
                CustomerEffectBonusProfit.EFFECT_ID_KEY => new CustomerEffectBonusProfit(effectData[1])
            };
        }
    }
}

