namespace Project.Gameplay
{
	using Utility;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


    public class LootBoxItemListUI : MonoBehaviour, IPoolingObjects
    {
        /// <summary>
        /// Base frame border
        /// </summary>
        [SerializeField] private Image _baseBorder;

        /// <summary>
        /// Image for item icon
        /// </summary>
        [SerializeField] private Image _iconImage;

        /// <summary>
        /// rarity text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _rarityText;

        /// <summary>
        /// rariries color
        /// </summary>
        [SerializeField] private List<WardrobeListUI.RarityColor> _raritiesColor;

        /// <summary>
        /// Object pool activate state
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Object pool activate state
        /// </summary>
        /// <returns> true: object active / false: object inactive </returns>
        public bool IsActive()
            => isActive;

        /// <summary>
        /// Set item ui elements
        /// </summary>
        /// <param name="icon"> item icon </param>
        /// <param name="rarity"> item rarity </param>
        public void SetElement(Sprite icon, string rarity)
        {
            _baseBorder.color = _raritiesColor.Find(x => string.Equals(x.RarityID, rarity)).Color;
            _rarityText.SetText(rarity);

            _iconImage.sprite = icon;
        }

        /// <summary>
        /// Set active pool object
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActive(bool state)
        {
            isActive = state;
            gameObject.SetActive(state);
        }
    }
}
