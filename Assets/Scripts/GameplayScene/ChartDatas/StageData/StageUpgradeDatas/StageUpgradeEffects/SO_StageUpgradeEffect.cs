namespace Project.Gameplay
{
    using UnityEngine;

    public abstract class SO_StageUpradeEffect : ScriptableObject
    {
        /// <summary>
        /// Upgrade effect code from database
        /// </summary>
        [SerializeField] private string _upgradeEffectID;
        
        /// <summary>
        /// Upgrade effect code from database
        /// </summary>
        public string UpgradeEffectID => _upgradeEffectID;

        /// <summary>
        /// Upgrade icon
        /// </summary>
        /// <returns></returns>
        public abstract Sprite GetIcon();

        /// <summary>
        /// Effect activation
        /// </summary>
        public abstract void ActivateEffect();
    }
}
