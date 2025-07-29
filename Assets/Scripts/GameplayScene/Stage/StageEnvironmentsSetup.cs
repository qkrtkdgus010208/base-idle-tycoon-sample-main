namespace Project.Gameplay
{
    using UnityEngine;


	public class StageEnvironmentsSetup : MonoBehaviour
	{
        [SerializeField] SaveData.SO_StageLoadData _stageLoadData;

        private void Start()
        {
            LoadingUIController.Instance.Loading();
            
            SetupStageEnvirontments();
            
            StageEventsManager.ScanPathMap?.Invoke();
            LoadingUIController.Instance.FinishLoading();
        }

        /// <summary>
        /// Setup environments in current played stage
        /// </summary>
        private void SetupStageEnvirontments()
        {
            Instantiate(_stageLoadData.CurrentStageEnvirontmentsData.Environments);
        }
	}
}
