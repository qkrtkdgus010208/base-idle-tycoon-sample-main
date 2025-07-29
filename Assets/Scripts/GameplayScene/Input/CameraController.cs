namespace Project.Gameplay
{
    using UnityEngine;


	public class CameraController : MonoBehaviour
	{
        /// <summary>
        /// Stage load data container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _loader;

        /// <summary>
        /// Stage data container
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stageDataCollection;

        /// <summary>
        /// Camera in gameplay scene
        /// </summary>
		[SerializeField] private Camera _cam;

        public Camera Camera => _cam;


        private void Awake()
        {
            _cam.orthographicSize = _stageDataCollection.GetStageDataById(_loader.CurrentStageID).CameraZoomSize;
        }
    }
}
