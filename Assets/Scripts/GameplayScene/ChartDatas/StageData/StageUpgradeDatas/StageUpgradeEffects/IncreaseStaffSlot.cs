namespace Project.Gameplay
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Upgrades/Upgrade Effects/Increase Staff Slot")]
    public class IncreaseStaffSlot : SO_StageUpradeEffect
    {
        /// <summary>
        /// Icon for this upgrade
        /// </summary>
        [SerializeField] private Sprite _icon;

        /// <summary>
        /// Staff type target
        /// </summary>
        [SerializeField] private StaffController.Staff_ID _staffType;

        /// <summary>
        /// Add more staff count
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
            => StageEventsManager.OnIncreaseStaffSlot?.Invoke(_staffType, _increaseAmount);
    }
}
