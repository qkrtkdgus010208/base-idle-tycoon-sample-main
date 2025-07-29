namespace Project.Gameplay
{
    public class CustomerEffectBonusProfit : AbstractCustomerEffect
    {
        /// <summary>
        /// Bonus profit code Effect code id
        /// </summary>
        public const string EFFECT_ID_KEY = "BP";
        private float _bonusMultiply;

        public CustomerEffectBonusProfit(string bonusMultiply)
        {
            _bonusMultiply = float.Parse(bonusMultiply); // parsing string bonusMultiply to profit bonus _bonusMultiply
        }

        /// <summary>
        /// Activate customer bonus profit effect
        /// </summary>
        /// <param name="customerController"> customer controller </param>
        public override void ActiveEffect(CustomerController customerController)
        {
            customerController.SetProfitBonusMultiply(_bonusMultiply); // set bonus profit to customer controller
        }
    }
}
