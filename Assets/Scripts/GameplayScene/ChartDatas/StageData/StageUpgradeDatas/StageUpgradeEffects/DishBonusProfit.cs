namespace Project.Gameplay
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Upgrades/Upgrade Effects/Dish Bonus Profit")]
    public class DishBonusProfit : SO_StageUpradeEffect
    {
        /// <summary>
        /// Dish data that get bonus
        /// </summary>
        [SerializeField] private SO_DishData _dishData;

        /// <summary>
        /// Bonus percentage 
        /// if the bonus 20% profit then the value is 20f
        /// </summary>
        [SerializeField] private float _bonusAmount;

        /// <summary>
        /// Get icon for this upgrade
        /// </summary>
        /// <returns> icon </returns>
        public override Sprite GetIcon()
            => _dishData.DishIcon;

        /// <summary>
        /// Activate the effect
        /// </summary>
        public override void ActivateEffect()
            => StageEventsManager.AddKitchenProfitBonus?.Invoke( // Add kitchen profit bonus
                _dishData.DishID, 
                _bonusAmount / Utility.StaticConstantDictionary.PERCENT);
    }
}
