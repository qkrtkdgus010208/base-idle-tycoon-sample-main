namespace Project.Gameplay
{
    public class DailyRewardGemsItem : AbstractDailyRewardReceiveItem
    {
        /// <summary>
        /// gems reward code id
        /// </summary>
        public const string STR_ITEM_KEY = "Gems";

        /// <summary>
        /// reward gems amount
        /// </summary>
        private int _gemsAmount;

        public DailyRewardGemsItem(string gemsAmount)
        {
            _gemsAmount = int.Parse(gemsAmount); // Parsing string to gems amount
        }

        /// <summary>
        /// Get Reward Amount
        /// </summary>
        /// <returns> Gems Amount </returns>
        public override long GetAmount()
            => _gemsAmount;

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
            => PlayerWallet.PlayerIncome(new CurrencyTransmission() { CurrencyID = Currency.ID.Gems, Amount = _gemsAmount }, true);
    }
}
