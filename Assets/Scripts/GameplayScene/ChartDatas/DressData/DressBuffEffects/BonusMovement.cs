namespace Project.Gameplay
{
    public class BonusMovement : AbstractDressBuffEffects
    {
        /// <summary>
        /// Effect code from database
        /// </summary>
        public const string BUFF_STR_KEY = "Movement";

        /// <summary>
        /// Bonus percentage 
        /// if the bonus 20% movement then the value is 20f
        /// </summary>
        private float _bonusPercentage;


        public BonusMovement(int bonusPercentage)
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
                StageEventsManager.AddMovementBonus( // add this movement bonus value
                    StaffController.Staff_ID.Manager, 
                    _bonusPercentage / Utility.StaticConstantDictionary.PERCENT
                    );
            else // deactive effect
                StageEventsManager.RemoveMovementBonus( // remove this movement bonus value
                    StaffController.Staff_ID.Manager, 
                    _bonusPercentage / Utility.StaticConstantDictionary.PERCENT
                    );
        }
    }
}
