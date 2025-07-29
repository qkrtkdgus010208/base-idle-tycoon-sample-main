namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


	[CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Upgrades/Stage Upgrade Effects Collections")]
	public class SO_StageUpgradeEffectsCollections : ScriptableObject
	{
        /// <summary>
        /// List of stage upgrade effect
        /// </summary>
        [SerializeField] private List<SO_StageUpradeEffect> _upgradeEffects;

        /// <summary>
        /// Get specific upgrade effect by id
        /// </summary>
        /// <param name="effectID"> target effect id </param>
        /// <returns> stage upgrade effect </returns>
        public SO_StageUpradeEffect GetUpgradeEffectByID(string effectID)
            => _upgradeEffects.Find(x => string.Equals(x.UpgradeEffectID, effectID));
    }
}
