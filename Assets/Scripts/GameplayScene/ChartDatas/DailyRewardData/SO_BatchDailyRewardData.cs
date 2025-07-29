namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEditor;
    using UnityEngine;


    [CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Daily Reward/Daily Reward Data")]
    public class SO_BatchDailyRewardData : SO_ChartData
    {
        /// <summary>
        /// Item code separator between item id and effect item amount
        /// </summary>
        private const char STR_ITEM_KEY_SEPARATOR = '_';

        /// <summary>
        /// List of daily reward data
        /// </summary>
        [SerializeField] private List<DailyRewardData> _rewardDatas;

        /// <summary>
        /// List of daily reward data
        /// </summary>
        public ReadOnlyCollection<DailyRewardData> RewardDatas => _rewardDatas.AsReadOnly();

        /// <summary>
        /// Initialize data from json data
        /// </summary>
        /// <param name="jsonData"></param>
        public override void Initialize(string jsonData)
        {
            // Convert json into list of daily reward data
            _rewardDatas = Utility.StaticReflection.DatabaseItemsParse<DailyRewardData>(jsonData);

            
            for (int i = 0; i < _rewardDatas.Count; i++) // for all reward data in _rewardDatas
            {
                // Parse RewardData code to reward item
                var strItemData = _rewardDatas[i].RewardData.Split(STR_ITEM_KEY_SEPARATOR);

                // example RewardData code : "Gems_10";
                // separate by '_'
                // RewardData first idx is item code
                // RewardData second idx is item amount
                _rewardDatas[i] = new DailyRewardData() // Generate new struct reward data
                {
                    DayIdx = _rewardDatas[i].DayIdx, // Copy day idx to this new data
                    RewardData = _rewardDatas[i].RewardData, // Copy reward data to this new data
                    RewardItem = strItemData[0] switch // Assign reward item based on reward data code
                    {
                        DailyRewardCoinItem.STR_ITEM_KEY => new DailyRewardCoinItem(strItemData[1]), // if reward code equals coin key, assign coin reward strItemData[1] as amount
                        DailyRewardGemsItem.STR_ITEM_KEY => new DailyRewardGemsItem(strItemData[1])  // if reward code equals gems key, assign gems reward strItemData[1] as amount
                    }
                };
            }
        }
    }
}
