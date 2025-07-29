namespace Project.Gameplay
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Upgrades/Upgrade Effects/Dish Reduce Process Time")]
    public class DishReduceProcessTime : SO_StageUpradeEffect
	{
        /// <summary>
        /// Dish data that get bonus
        /// </summary>
        [SerializeField] private SO_DishData _dishData;

        /// <summary>
        /// Bonus percentage 
        /// if the bonus 20% reduce time then the value is 20f
        /// </summary>
        [SerializeField] private float _reduceAmount;

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
            => StageEventsManager.AddKitchenReduceTime?.Invoke(
                _dishData.DishID, 
                _reduceAmount / Utility.StaticConstantDictionary.PERCENT);
    }
}
