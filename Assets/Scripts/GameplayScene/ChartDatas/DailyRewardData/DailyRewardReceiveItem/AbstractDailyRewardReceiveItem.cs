namespace Project.Gameplay
{
	/// <summary>
	/// Daily reward item object
	/// </summary>
	public abstract class AbstractDailyRewardReceiveItem
	{
		public abstract string GetRewardID();
		public abstract long GetAmount();
		public abstract void ReceiveItem();
	}
}
