namespace Project.Gameplay
{
    using Project.Utility;
    using SaveData;
    using System;
	using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


	public class WardrobeManager : MonoBehaviour
	{
        /// <summary>
        /// Dress data container
        /// </summary>
        [SerializeField] private SO_BatchDressData _dressBatchData;
        
        /// <summary>
        /// Dress object data container
        /// </summary>
        [SerializeField] private SO_DressObjectDataCollection _dressObjectCollection;

        /// <summary>
        /// Wardrobe UI
        /// </summary>
        [SerializeField] private GameObject _ui;
        
        /// <summary>
        /// Button to open wardrobe
        /// </summary>
        [SerializeField] private Button _openWardrobeUIButton;

        /// <summary>
        /// Button to close wardrobe
        /// </summary>
        [SerializeField] private Button _closeWardrobeUIButton;

        /// <summary>
        /// Button to go to shop
        /// </summary>
        [SerializeField] private Button _gotoShopButton;

        /// <summary>
        /// Display character
        /// </summary>
        [SerializeField] private Image _characterIcon;

        /// <summary>
        /// Current used dress buff effect description text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _boostDescText;

        /// <summary>
        /// wardrobe list item parent
        /// </summary>
        [SerializeField] private Transform _listParent;

        /// <summary>
        /// wardrobe list item template
        /// </summary>
        [SerializeField] private WardrobeListUI _wardrobeListTemplate;

        /// <summary>
        /// Events to add new user dress data
        /// </summary>
		public static Action<List<string>> AddNewDressData;

        /// <summary>
        /// Events to get specific user dress data
        /// </summary>
        public static Func<string, UserWardrobeData> GetDressDataByID;

        /// <summary>
        /// Events to get specific dress object
        /// </summary>
        public static Func<string, DressObject> GetDressObjectByID;

        /// <summary>
        /// wardrobe item pool
        /// </summary>
        private ClassPooling<WardrobeListUI> _dressListUIPool;

        /// <summary>
        /// temporary for added new dress data
        /// </summary>
        private List<UserWardrobeData> tempNewDressData = new List<UserWardrobeData>();

        /// <summary>
        /// Current list ui that handle current used dress
        /// </summary>
        private WardrobeListUI currentUsedDressUI;


        private void Awake()
        {
            AddNewDressData = OnAddNewDressData;
            GetDressDataByID = OnGetDressDataByID;
            GetDressObjectByID = OnGetDressObjectByID;

            _openWardrobeUIButton.onClick.AddListener(() => SetActiveUI(true));
            _closeWardrobeUIButton.onClick.AddListener(() => SetActiveUI(false));
            _gotoShopButton.onClick.AddListener(() =>
            {
                SetActiveUI(false);
                ShopManager.OpenShopUI?.Invoke();
            });

            _dressListUIPool = new ClassPooling<WardrobeListUI>(()=> Instantiate(_wardrobeListTemplate, _listParent));
        }

        /// <summary>
        /// Events when player get new dress data
        /// </summary>
        /// <param name="dressID"> list of new dress id </param>
        private void OnAddNewDressData(List<string> dressID)
        {
            tempNewDressData.Clear();

            for (int i = 0; i < dressID.Count; i++)
            {
                if (UserWardrobeDataManager.Instance.GetDressDataByID(dressID[i]) == null)
                    tempNewDressData.Add(new UserWardrobeData()
                    {
                        inDate = string.Empty,
                        DressID = dressID[i],
                        DressTotalXP = 0
                    });
            }

            if (tempNewDressData.Count > 0)
                UserWardrobeDataManager.Instance.AddDressData(tempNewDressData);
        }

        /// <summary>
        /// Get specific user dress data
        /// </summary>
        /// <param name="dressID"> target dress id </param>
        /// <returns> user dress data </returns>
        private UserWardrobeData OnGetDressDataByID(string dressID)
        {
            if (string.IsNullOrEmpty(dressID))
                dressID = StaticConstantDictionary.DEFAULT_DRESS_ID;

            return UserWardrobeDataManager.Instance.GetDressDataByID(dressID);
        }

        /// <summary>
        /// Get specific dress object
        /// </summary>
        /// <param name="dressID"> target dress id </param>
        /// <returns> dress object </returns>
        private DressObject OnGetDressObjectByID(string dressID)
        {
            if (string.IsNullOrEmpty(dressID))
                dressID = StaticConstantDictionary.DEFAULT_DRESS_ID;

            return _dressBatchData.GetDressDataByID(dressID);
        }

        /// <summary>
        /// Set active wardrobe ui
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActiveUI(bool state)
        {
            if (state)
            {
                var userDress = UserWardrobeDataManager.Instance.UserWardrobeDatas;
                
                foreach (var dressID in userDress.Keys)
                    SetUIList(dressID);

                SetUIList(StaticConstantDictionary.DEFAULT_DRESS_ID);
            }
            else
                _dressListUIPool.ClearActiveObj();

            _ui.SetActive(state);
        }

        /// <summary>
        /// Initialize dress list ui
        /// </summary>
        /// <param name="dressID"> dress id that will initialized dress list ui handle </param>
        private void SetUIList(string dressID)
        {
            var listUI = _dressListUIPool.GetFromPool();
            var dress = _dressBatchData.GetDressDataByID(dressID);
            
            listUI.SetElement(
                dress.SpriteObject, 
                dress.Rarity.ToLower(), 
                () => EquipDress(dress, listUI)
                );

            var usedDressID = string.IsNullOrEmpty(UserDataManager.Instance.UsedDressID) ?
                StaticConstantDictionary.DEFAULT_DRESS_ID : UserDataManager.Instance.UsedDressID;

            if (usedDressID == dressID)
            {
                listUI.SetEquip(true);
                currentUsedDressUI = listUI;

                _characterIcon.sprite = dress.SpriteObject;
                _boostDescText.SetText(dress.EffectBonusDescription);
            }
            else
                listUI.SetEquip(false);

            listUI.SetActive(true);
        }

        /// <summary>
        /// On wear dress
        /// </summary>
        /// <param name="dress"> dress object </param>
        /// <param name="listUI"> list ui that handle dress data </param>
        private void EquipDress(DressObject dress, WardrobeListUI listUI)
        {
            UserDataManager.Instance.UpdateUsedDressID(dress.DressID, true);
            
            _characterIcon.sprite = dress.SpriteObject;
            _boostDescText.SetText(dress.EffectBonusDescription);

            currentUsedDressUI.SetEquip(false);
            currentUsedDressUI = listUI;
            currentUsedDressUI.SetEquip(true);

            StageEventsManager.OnChangeDress?.Invoke(OnGetDressObjectByID(dress.DressID));
        }
    }
}
