namespace Project.Gameplay
{
    using System;
    using UnityEngine;


    public class KitchenTable : MonoBehaviour
    {
        /// <summary>
        /// Table object sprite
        /// </summary>
        [SerializeField] private GameObject _tableSprite;

        /// <summary>
        /// Staff stop point
        /// </summary>
        [SerializeField] private Transform _pathPoint;

        /// <summary>
        /// Dish on process progress bar
        /// </summary>
        [SerializeField] private ProgressBar _progressBar;

        /// <summary>
        /// Staff stop point
        /// </summary>
        public Vector2 PointPosition => _pathPoint.position;

        /// <summary>
        /// callback when dish on process finish
        /// </summary>
        private Action OnProcessDone;

        /// <summary>
        /// is table active state
        /// true: table is active
        /// false: table still locked
        /// </summary>
        private bool isTableActive;

        /// <summary>
        /// is table active state
        /// true: table is active
        /// false: table still locked
        /// </summary>
        public bool IsTableActive => isTableActive;

        /// <summary>
        /// is table busy state
        /// true: there is staff operate this table
        /// false: available for staff to operate
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// is table busy state
        /// true: there is staff operate this table
        /// false: available for staff to operate
        /// </summary>
        public bool IsBusy => isBusy;

        /// <summary>
        /// Set active table
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActiveTable(bool state)
        {
            isTableActive = state;
            _tableSprite.SetActive(state);

            if (state)
                StageEventsManager.CheckingFreeStaff?.Invoke();
        }

        /// <summary>
        /// set to busy
        /// </summary>
        public void SetBusy()
        {
            isBusy = true;
        }

        /// <summary>
        /// When staff operate the table
        /// </summary>
        /// <param name="processTime"> long dish process time </param>
        /// <param name="onProcessDone"> callback after dish on process finish </param>
        public void ProcessTable(float processTime, Action onProcessDone)
        {
            OnProcessDone = onProcessDone;
            _progressBar.StartProgress(processTime, OnProcessFinish);
        }

        /// <summary>
        /// After process finish
        /// </summary>
        private void OnProcessFinish()
        {
            OnProcessDone?.Invoke();
            isBusy = false;

            StageEventsManager.CheckingFreeStaff?.Invoke();
        }
    }
}