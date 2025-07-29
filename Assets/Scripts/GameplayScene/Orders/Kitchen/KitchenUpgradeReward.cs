namespace Project.Gameplay
{
    /// <summary>
    /// Kitchen upgrade reward object
    /// </summary>
    public abstract class AbstractKitchenUpgradeReward
    {
        /// <summary>
        /// Player recieve reward
        /// </summary>
        public abstract void ReceiveReward();
    }

    public class KitchenUpgradeRewardCurrency : AbstractKitchenUpgradeReward
    {
        /// <summary>
        /// Currency reward code key
        /// </summary>
        public const string REWARD_TYPE_KEY = "Currency";

        /// <summary>
        /// currency id that will player get
        /// </summary>
        private Currency.ID _currencyID;

        /// <summary>
        /// reward amount that will player get
        /// </summary>
        private long _amount;

        public KitchenUpgradeRewardCurrency(string strID, string amount)
        {
            System.Enum.TryParse(strID, out _currencyID);
            _amount = long.Parse(amount);
        }

        /// <summary>
        /// Player recieve reward
        /// </summary>
        public override void ReceiveReward()
        {
            // save reward first
            switch (_currencyID)
            {
                case Currency.ID.Coins:
                    StageManager.Instance.PlayerCoinIncome(_amount, true);
                    break;
                case Currency.ID.Gems:
                    PlayerWallet.PlayerIncome(
                        new CurrencyTransmission() { CurrencyID = _currencyID, Amount = _amount }, true);
                    break;
            }

            // show confirmation UI
            RewardConfirmationController.ShowRewardConfirmation?.Invoke( 
                Utility.StaticImageDictionary.Instance.GetImageSpriteByID(_currencyID.ToString()),
                Utility.StaticCurrencyStringConverison.GetString(_amount),
                null
                );
        }
    }

    public class KitchenUpgradeRewardLootbox : AbstractKitchenUpgradeReward
    {
        /// <summary>
        /// Lootbox reward code key
        /// </summary>
        public const string REWARD_TYPE_KEY = "LootBox";

        /// <summary>
        /// lootbox id that will player get
        /// </summary>
        private string _lootBoxID;

        /// <summary>
        /// reward amount that will player get
        /// </summary>
        private int _amount;


        public KitchenUpgradeRewardLootbox(string lootBoxID, string amount)
        {
            _lootBoxID = lootBoxID;
            _amount = int.Parse(amount);
        }

        /// <summary>
        /// Player recieve reward
        /// </summary>
        public override void ReceiveReward()
        {
            for (int i = 0; i < _amount; i++)
            {
                LootBoxManager.OpenLootBoxAfterConfirmation?.Invoke(_lootBoxID); // save reward first

                RewardConfirmationController.ShowRewardConfirmation?.Invoke( // show confirmation UI
                    Utility.StaticImageDictionary.Instance.GetImageSpriteByID(_lootBoxID),
                    _amount.ToString(),
                    LootBoxManager.ConfirmedToOpenLootboxUI
                    );
            }
        }
    }
}
