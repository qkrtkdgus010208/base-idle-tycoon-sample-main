namespace Project.Gameplay
{
    using Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class LootBoxManager : MonoBehaviour
	{
        /// <summary>
        /// Lootbox probability data container
        /// </summary>
        [SerializeField] private SO_LootBoxProbabilityData _lootBoxProbabilityData;

        /// <summary>
        /// lootbox data container
        /// </summary>
        [SerializeField] private SO_BatchLootBoxData _lootBoxData;

        /// <summary>
        /// Open lootbox UI
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// Full screen size button
        /// to reveal obtained item from lootbox
        /// </summary>
        [SerializeField] private Button _openLootBoxItemButton;

        /// <summary>
        /// Image for current loobox opened
        /// </summary>
        [SerializeField] private Image _lootboxObj;

        /// <summary>
        /// obtained items count panel
        /// </summary>
        [SerializeField] private GameObject _itemsCountIndicator;

        /// <summary>
        /// obtained items count text
        /// </summary>
        [SerializeField] private TextMeshProUGUI _itemsCountText;

        /// <summary>
        /// lootbox item obtained template
        /// </summary>
        [SerializeField] private LootBoxItemListUI _itemListUITemplate;

        /// <summary>
        /// lootbox list item obtained parent
        /// </summary>
        [SerializeField] private Transform _itemListParent;

        /// <summary>
        /// item list pool
        /// </summary>
        private ClassPooling<LootBoxItemListUI> itemListUIPool;

        /// <summary>
        /// Event to open lootbox
        /// string: lootbox id
        /// </summary>
        public static Action<string> OpenLootBox;

        /// <summary>
        /// Event to open lootbox after confirmation UI
        /// string: lootbox id
        /// </summary>
        public static Action<string> OpenLootBoxAfterConfirmation;

        /// <summary>
        /// Event to confirm to open lootbox UI
        /// </summary>
        public static Action ConfirmedToOpenLootboxUI;

        /// <summary>
        /// open lootbox counter
        /// </summary>
        private int openedLootboxCounter;

        /// <summary>
        /// opened lootbox that alreade revealed counter
        /// </summary>
        private int currentOpenLootboxCounterIdx = 1;

        /// <summary>
        /// Is all item obtained from lootbox already show up
        /// </summary>
        private bool isAllItemOpened;


        private Dictionary<string, List<string>> lootboxObtainedItemsToShowAfterConfimation = new Dictionary<string, List<string>>();


        private void Awake()
        {
            OpenLootBox = (lootboxID) => OnOpenLootBox(lootboxID, true);
            OpenLootBoxAfterConfirmation = (lootboxID) => OnOpenLootBox(lootboxID, false);
            ConfirmedToOpenLootboxUI = OnConfirmedToOpenLootboxUI;

            itemListUIPool = new ClassPooling<LootBoxItemListUI>(() => Instantiate(_itemListUITemplate, _itemListParent));
        }

        /// <summary>
        /// set active open lootbox ui
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveUI(bool state)
        {
            if (!state)
                lootboxObtainedItemsToShowAfterConfimation.Clear();

            _ui.SetActive(state);
        }

        /// <summary>
        /// Open lootbox action
        /// </summary>
        /// <param name="lootBoxID"> lootbox id </param>
        private void OnOpenLootBox(string lootBoxID, bool showGachaUIImmediately)
        {
            var lootBox = _lootBoxData.GetLootBoxDataByID(lootBoxID);

            openedLootboxCounter++; // add counter for lootbox obtained item to reveal
            int currentIdx = openedLootboxCounter;

            _lootBoxProbabilityData.DrawGacha(
                UnityEngine.Random.Range(lootBox.LootboxData.MinItem, lootBox.LootboxData.MaxItem),
                (itemsList) => OnGetLootBoxItems(lootBoxID, currentIdx, itemsList, showGachaUIImmediately));
        }

        /// <summary>
        /// After successful get random from probability
        /// </summary>
        /// <param name="lootBoxID"> lootbox id </param>
        /// <param name="counterIdx"> queue idx to open </param>
        /// <param name="itemsID"> obtained items from lootbox </param>
        private void OnGetLootBoxItems(string lootBoxID, int counterIdx, List<string> itemsID, bool showGachaUIImmediately)
        {
            itemsID.Sort();
            WardrobeManager.AddNewDressData(itemsID); // save obtained item before show to player

            if (showGachaUIImmediately)
                StartCoroutine(RevealLootboxItem(lootBoxID, counterIdx, itemsID));
            else
                lootboxObtainedItemsToShowAfterConfimation.Add(lootBoxID, itemsID);
        }

        /// <summary>
        /// After confirmed to open lootbox ui
        /// </summary>
        private void OnConfirmedToOpenLootboxUI()
        {
            Debug.Log("Xcute :" + lootboxObtainedItemsToShowAfterConfimation.Count);

            if (lootboxObtainedItemsToShowAfterConfimation.Count <= 0)
                return;

            int showQueueIdx = openedLootboxCounter - lootboxObtainedItemsToShowAfterConfimation.Count;
            foreach (var lootboxToShow in lootboxObtainedItemsToShowAfterConfimation)
            {
                showQueueIdx++;
                StartCoroutine(RevealLootboxItem(lootboxToShow.Key, showQueueIdx, lootboxToShow.Value));
            }
        }

        /// <summary>
        /// Reveal lootbox item
        /// </summary>
        /// <param name="lootBoxID"> lootbox id </param>
        /// <param name="counterIdx"> queue idx to open </param>
        /// <param name="listItems"> obtained items from lootbox </param>
        /// <returns></returns>
        private IEnumerator RevealLootboxItem(string lootBoxID, int counterIdx, List<string> listItems)
        {
            yield return new WaitUntil(() => counterIdx == currentOpenLootboxCounterIdx); // await FIFO
            
            isAllItemOpened = false;

            _lootboxObj.sprite = _lootBoxData.GetLootBoxDataByID(lootBoxID).CloseSpriteObject;
            
            _itemsCountText.SetText(listItems.Count.ToString());
            _itemsCountIndicator.SetActive(true);

            _openLootBoxItemButton.onClick.RemoveAllListeners();
            _openLootBoxItemButton.onClick.AddListener(() => OpenLootBoxItem(lootBoxID, counterIdx, listItems));

            SetActiveUI(true);
        }

        /// <summary>
        /// Show all item obtained from lootbox
        /// </summary>
        /// <param name="lootBoxID"> lootbox id that opened </param>
        /// <param name="counterIdx"> current opened lootbox index </param>
        /// <param name="listItems"> list of obtained items from lootbox </param>
        private void OpenLootBoxItem(string lootBoxID, int counterIdx, List<string> listItems)
        {
            if (isAllItemOpened)
            {
                currentOpenLootboxCounterIdx = counterIdx + 1;
                itemListUIPool.ClearActiveObj();
                SetActiveUI(currentOpenLootboxCounterIdx < openedLootboxCounter);
                return;
            }

            _lootboxObj.sprite = _lootBoxData.GetLootBoxDataByID(lootBoxID).OpenSpriteObject;
            _itemsCountIndicator.SetActive(false);

            foreach (var item in listItems)
            {
                var dressObject = WardrobeManager.GetDressObjectByID(item);

                var itemUI = itemListUIPool.GetFromPool();
                itemUI.SetElement(
                    dressObject.SpriteObject,
                    dressObject.Rarity.ToLower()
                    );
                itemUI.SetActive(true);
            }

            isAllItemOpened = true;
        }
    }
}
