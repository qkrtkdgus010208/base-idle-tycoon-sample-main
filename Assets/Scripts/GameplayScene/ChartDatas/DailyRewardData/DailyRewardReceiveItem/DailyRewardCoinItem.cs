namespace Project.Gameplay
{
	public class DailyRewardCoinItem : AbstractDailyRewardReceiveItem
	{
        /// <summary>
        /// coins reward code id
        /// </summary>
		public const string STR_ITEM_KEY = "Coins";

        /// <summary>
        /// reward coins amount
        /// </summary>
		private long _coinAmount;

		public DailyRewardCoinItem(string coinAmount)
        {
			_coinAmount = long.Parse(coinAmount);
        }

        /// <summary>
        /// Get Reward Amount
        /// </summary>
        /// <returns> Coins Amount </returns>
        public override long GetAmount() 
            => _coinAmount;

        /// <summary>
        /// Get Reward Item Code ID
        /// </summary>
        /// <returns> Reward Item ID </returns>
        public override string GetRewardID()
            => STR_ITEM_KEY;

        /// <summary>
        /// Give item to player
        /// </summary>
        public override void ReceiveItem()
            => StageManager.Instance.PlayerCoinIncome(_coinAmount, true);
	}
}
