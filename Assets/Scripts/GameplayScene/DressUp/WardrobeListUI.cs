namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;


	public class WardrobeListUI : MonoBehaviour, Utility.IPoolingObjects
	{
        /// <summary>
        /// button to wear this dress
        /// </summary>
        [SerializeField] private Button _wearDressButton;

        /// <summary>
        /// Image to base frame border
        /// </summary>
        [SerializeField] private Image _baseBorder;

        /// <summary>
        /// Image to dress icon
        /// </summary>
        [SerializeField] private Image _iconImage;

        /// <summary>
        /// Rarity text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _rarityText;

        /// <summary>
        /// Rarities color
        /// </summary>
        [SerializeField] private List<RarityColor> _raritiesColor;

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
        /// Set to wear this dress
        /// </summary>
        /// <param name="isEquip"> true: equiped / false: can equip </param>
        public void SetEquip(bool isEquip)
        {
            _wearDressButton.interactable = !isEquip;
        }

        /// <summary>
        /// Set elements
        /// </summary>
        /// <param name="dressIcon"> dress icon </param>
        /// <param name="rarity"> dress rarity </param>
        /// <param name="onWeardress"> on click button action </param>
        public void SetElement(Sprite dressIcon, string rarity, Action onWeardress)
        {
            _baseBorder.color = _raritiesColor.Find(x => string.Equals(x.RarityID, rarity)).Color;
            _rarityText.SetText(rarity);
            _iconImage.sprite = dressIcon;

            _wearDressButton.onClick.AddListener(() => onWeardress?.Invoke());
        }

        /// <summary>
        /// Set active object pool
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActive(bool state)
        {
            isActive = state;
            gameObject.SetActive(state);
        }

        /// <summary>
        /// Rarity color
        /// </summary>
        [Serializable]
        public struct RarityColor
        {
            public string RarityID;
            public Color Color;
        }
    }
}
