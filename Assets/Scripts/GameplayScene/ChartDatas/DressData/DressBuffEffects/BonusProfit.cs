namespace Project.Gameplay
{
    public class BonusProfit : AbstractDressBuffEffects
    {
        /// <summary>
        /// Effect code from database
        /// </summary>
        public const string BUFF_STR_KEY = "Profit";

        /// <summary>
        /// Bonus percentage 
        /// if the bonus 20% profit then the value is 20f
        /// </summary>
        private float _bonusPercentage;


        public BonusProfit(int bonusPercentage)
        {
            _bonusPercentage = bonusPercentage; // Assign _bonusPercentage
        }

        /// <summary>
        /// Set this effect active state
        /// </summary>
        /// <param name="state"> active effect state </param>
        public override void SetActiveEffect(bool state)
        {
            if (state) // set active effect
                StageEventsManager.AddGeneralProfitBonus( // add this profit bonus value
                    _bonusPercentage / Utility.StaticConstantDictionary.PERCENT);
            else // deactive
                StageEventsManager.RemoveGeneralProfitBonus( // remove this profit bonus value
                    _bonusPercentage / Utility.StaticConstantDictionary.PERCENT);
        }
    }
}
