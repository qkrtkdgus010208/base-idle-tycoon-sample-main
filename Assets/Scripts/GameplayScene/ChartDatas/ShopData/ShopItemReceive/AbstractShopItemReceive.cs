namespace Project.Gameplay
{
	/// <summary>
	/// Shop item object that will player recieve after purchased sale item
	/// </summary>
	public abstract class AbstractShopItemReceive
	{
		/// <summary>
		/// Give item to player
		/// </summary>
		public abstract void ReceiveItem();
	}
}
