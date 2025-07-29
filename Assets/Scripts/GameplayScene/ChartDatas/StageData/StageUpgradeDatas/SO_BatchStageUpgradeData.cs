namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;
    


	[CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Upgrades/Stage Upgrade Data")]
	public class SO_BatchStageUpgradeData : SO_ChartData
	{
        /// <summary>
        /// Stage id for this upgrade data
        /// </summary>
        [SerializeField] private string _stageID;

        /// <summary>
        /// Stage id for this upgrade data
        /// </summary>
        public string StageID => _stageID;

        /// <summary>
        /// List of upgrade data in this stage
        /// </summary>
        [SerializeField] private List<StageUpgradeData> _upgradeDatas;

        /// <summary>
        /// Count of upgrade data in list
        /// </summary>
        public int UpgradeDataCount => _upgradeDatas.Count;

        /// <summary>
        /// Initialize upgrade data
        /// </summary>
        /// <param name="jsonData"> json data </param>
        public override void Initialize(string jsonData)
        {
            _upgradeDatas = Utility.StaticReflection.DatabaseItemsParse<StageUpgradeData>(jsonData); // parse json data into list of upgrade data

            // sort upgrade data by the price
            // more lower the price more low the index
            for (int i = 0; i < _upgradeDatas.Count; i++)
            {
                _upgradeDatas[i].Level = i + 1;

                for (int j = 0; j < i; j++)
                {
                    if (_upgradeDatas[j].Price > _upgradeDatas[i].Price)
                    {
                        var temp = _upgradeDatas[j];

                        _upgradeDatas[j] = _upgradeDatas[i];
                        _upgradeDatas[j].Level = j + 1;

                        _upgradeDatas[i] = temp;
                        _upgradeDatas[i].Level = i + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Get specific upgrade data by idx
        /// </summary>
        /// <param name="idx"> upgrade data idx </param>
        /// <returns> Stage Upgrade Data </returns>
        public StageUpgradeData GetUpgradeDataByIdx(int idx)
            => (idx < 0 || idx >= UpgradeDataCount) ?
                new StageUpgradeData() : _upgradeDatas[idx];
    }
}
