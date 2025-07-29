namespace Project.Gameplay
{
    public class CustomerEffectNone : AbstractCustomerEffect
    {
        /// <summary>
        /// Activate none effect
        /// </summary>
        /// <param name="customerController"> customer object controller </param>
        public override void ActiveEffect(CustomerController customerController)
        {
            return; // None effect
        }
    }
}
