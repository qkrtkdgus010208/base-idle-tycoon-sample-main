namespace Project.Gameplay
{
	/// <summary>
	/// Customer effect abstact class
	/// </summary>
	public abstract class AbstractCustomerEffect
	{
		/// <summary>
		/// Activate customer effect
		/// Child function
		/// </summary>
		/// <param name="customerController"> Customer controller that have this effect in customer data </param>
		public abstract void ActiveEffect(CustomerController customerController);
	}
}
