namespace Project.Gameplay
{
    /// <summary>
    /// Mail reward item object
    /// </summary>
	public abstract class AbstractMailReceiveReward
	{
		public abstract void ReceiveReward();
	}

    /// <summary>
    /// currency mail reward
    /// </summary>
    public class MailRewardCurrency : AbstractMailReceiveReward
    {
        /// <summary>
        /// Currency mail reward code key
        /// </summary>
        public const string REWARD_TYPE_KEY = "Currency";

        private Currency.ID _currencyID;
        private long _amount;

        public MailRewardCurrency(string strID, long amount)
        {
            System.Enum.TryParse(strID, out _currencyID);
            _amount = amount;
        }

        /// <summary>
        /// receive reward
        /// </summary>
        public override void ReceiveReward()
        {
            switch (_currencyID) {
                case Currency.ID.Coins:
                    StageManager.Instance.PlayerCoinIncome(_amount, true);
                    break;
                case Currency.ID.Gems:
                    PlayerWallet.PlayerIncome(
                        new CurrencyTransmission() { CurrencyID = _currencyID, Amount = _amount }, true);
                    break;
            }
        }
    }

    /// <summary>
    /// lootbox mail reward
    /// </summary>
    public class MailRewardLootbox : AbstractMailReceiveReward
    {
        /// <summary>
        /// Lootbox mail reward code key
        /// </summary>
        public const string REWARD_TYPE_KEY = "LootBox";

        private string _lootBoxID;
        private int _count;


        public MailRewardLootbox(string lootBoxID, int count)
        {
            _lootBoxID = lootBoxID;
            _count = count;
        }

        /// <summary>
        /// receive reward
        /// </summary>
        public override void ReceiveReward()
        {
            for (int i = 0; i < _count; i++)
                LootBoxManager.OpenLootBox?.Invoke(_lootBoxID);
        }
    }
}
