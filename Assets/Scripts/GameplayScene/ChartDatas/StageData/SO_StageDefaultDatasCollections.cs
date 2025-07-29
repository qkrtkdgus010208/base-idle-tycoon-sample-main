namespace Project.Gameplay
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;


	[CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Objects/Stage Data/Stage Default Datas Collections")]
	public class SO_StageDefaultDatasCollections : SO_ChartData
	{
        [SerializeField] private List<StageDefaultData> _stageDatas;
        public ReadOnlyCollection<StageDefaultData> StageDataCollection => _stageDatas.AsReadOnly();

        public override void Initialize(string jsonData)
        {
            _stageDatas = Utility.StaticReflection.DatabaseItemsParse<StageDefaultData>(jsonData);
        }

        /// <summary>
        /// Look up stage data by stage id
        /// </summary>
        /// <param name="stageID"> stage id </param>
        /// <param name="nextStageAfter"> get stage data +n after stageID </param>
        /// <returns> Stage Data </returns>
        public StageDefaultData GetStageDataById(string stageID, int nextStageAfter = 0)
        {
            for (int i = 0; i < _stageDatas.Count - nextStageAfter; i++)
                if (string.Equals(_stageDatas[i].StageID, stageID))
                    return _stageDatas[i + nextStageAfter];

            return new StageDefaultData();
        }

        /// <summary>
        /// Look up stage data idx by stage id
        /// </summary>
        /// <param name="stageID"> stage id </param>
        /// <returns> Stage Idx </returns>
        public int GetStageIdxById(string stageID)
        {
            for (int i = 0; i < _stageDatas.Count; i++)
                if (string.Equals(_stageDatas[i].StageID, stageID))
                    return i;

            return _stageDatas.Count;
        }
    }
}
