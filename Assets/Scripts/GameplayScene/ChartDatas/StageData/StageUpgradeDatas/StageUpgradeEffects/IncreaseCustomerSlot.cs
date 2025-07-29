namespace Project.Gameplay
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Upgrades/Upgrade Effects/Increase Customer Slot")]
    public class IncreaseCustomerSlot : SO_StageUpradeEffect
    {
        /// <summary>
        /// Icon for this upgrade
        /// </summary>
        [SerializeField] private Sprite _icon;
        
        /// <summary>
        /// Add more customer count
        /// </summary>
        [SerializeField] private int _increaseAmount;

        /// <summary>
        /// Get icon for this upgrade
        /// </summary>
        /// <returns> icon </returns>
        public override Sprite GetIcon()
            => _icon;

        /// <summary>
        /// Activate the effect
        /// </summary>
        public override void ActivateEffect()
            => StageEventsManager.OnIncreaseCustomerSlot?.Invoke(_increaseAmount);
    }
}
