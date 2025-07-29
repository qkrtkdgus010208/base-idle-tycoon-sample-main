namespace Project.Gameplay
{
    /// <summary>
    /// Customer order data
    /// </summary>
    public class OrderData
    {
        /// <summary>
        /// Dish data for this order
        /// </summary>
        public SO_DishData DishData;

        /// <summary>
        /// This order total profit for player income
        /// </summary>
        public long Profit;

        /// <summary>
        /// Customer idx
        /// Customer can order more than one
        /// Then some order data may have same number of customer idx 
        /// </summary>
        public int CustomerIdx;

        /// <summary>
        /// Customer bonus profits 
        /// If customer have bonus profits effect 
        /// </summary>
        public float CustomerBonusProfits;
    }
}