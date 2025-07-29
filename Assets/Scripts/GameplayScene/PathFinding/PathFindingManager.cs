namespace Project.Gameplay
{
    using UnityEngine;


	public class PathFindingManager : MonoBehaviour
	{
        /// <summary>
        /// A* path finding
        /// </summary>
		[SerializeField] private AstarPath _pathFinding;


        private void Awake()
        {
            StageEventsManager.ScanPathMap += ScanPath;
        }

        private void OnDestroy()
        {
            StageEventsManager.ScanPathMap -= ScanPath;
        }

        /// <summary>
        /// Scan A* path finding
        /// </summary>
        private void ScanPath()
        {
            _pathFinding.Scan(_pathFinding.graphs[0]);
        }
    }
}
