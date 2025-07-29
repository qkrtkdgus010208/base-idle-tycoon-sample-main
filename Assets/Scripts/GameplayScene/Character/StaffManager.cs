namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


    public class StaffManager : MonoBehaviour
    {
        /// <summary>
        /// stage object loader
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _stageLoadData;

        /// <summary>
        /// Stage starter (default) data collection
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stageDatas;

        /// <summary>
        /// list of staff
        /// </summary>
        private List<StaffController> _staffs = new List<StaffController>();

        /// <summary>
        /// staff manager template
        /// </summary>
        private StaffController _managerTemplate;

        /// <summary>
        /// staff helper template
        /// </summary>
        private StaffController _staffHelperTemplate;

        /// <summary>
        /// list of staff idle point
        /// </summary>
        private List<StaffPoint> _staffPointsList = new List<StaffPoint>();

        private void Awake()
        {
            StageEventsManager.CheckingFreeStaff += CheckingForFreeStaff;
            StageEventsManager.OnIncreaseStaffSlot += AddMoreStaff;
        }

        private void OnDestroy()
        {
            StageEventsManager.CheckingFreeStaff -= CheckingForFreeStaff;
            StageEventsManager.OnIncreaseStaffSlot -= AddMoreStaff;
        }

        private void Start()
        {
            LoadingUIController.Instance.Loading(); // Start loading

            var loadData = _stageLoadData.CurrentStageStaffData; // Get staff data in current stage to load

            _managerTemplate = loadData.ManagerTemplate; // Get staff manager template prefab from stage object loader
            _staffHelperTemplate = loadData.StaffHelperTemplate; // Get staff helper template prefab from stage object loader

            for (int i = 0; i < loadData.StaffPoints.Length; i++) // foreach staff point in stage data loader
                _staffPointsList.Add (new StaffPoint() { // generate new staff point then adding to list
                    PositionPoints = Instantiate(loadData.StaffPoints[i]).position, // Instantiate stage point prefabs from stage object loader
                    isAvailable = true // Set available
                });

            AddMoreStaff(StaffController.Staff_ID.Manager, 1); // Spawn 1 manager
            AddMoreStaff(StaffController.Staff_ID.StaffHelper, _stageDatas.GetStageDataById(loadData.StageID).StartingStaffHelperCount); // Spawn starter (default) helper in this stage if any

            StageEventsManager.ScanPathMap?.Invoke(); // Scan path finding map
            LoadingUIController.Instance.FinishLoading(); // End loading
        }

        /// <summary>
        /// Checking if there is available staff
        /// Do for all available staff, get that staff task to do if any task to do
        /// </summary>
        private void CheckingForFreeStaff()
        {
            // For every staff that not busy,
            // Try to get task to do
            foreach (StaffController staff in _staffs)
                if (!staff.IsBusy) StageEventsManager.GetTaskToDo(staff);
        }

        /// <summary>
        /// Adding more staff to field
        /// </summary>
        /// <param name="staffID"> staff id that will added </param>
        /// <param name="count"> staff added count </param>
        private void AddMoreStaff(StaffController.Staff_ID staffID, int count)
        {
            for (int i = 0; i < count; i++) // add more staff based on count
            {
                var staffPoint = GetAvailableStaffPoint(); // looking for available idle staff point
                var staff = Instantiate(staffID switch {
                        StaffController.Staff_ID.Manager => _managerTemplate, // adding more manager
                        StaffController.Staff_ID.StaffHelper => _staffHelperTemplate // adding more staff helper
                    },
                    staffPoint.PositionPoints, // spawn at staff point
                    Quaternion.identity // set default rotation
                );

                staff.SetStaffPoint(staffPoint); // set staff idle point
                _staffs.Add(staff); // add new staff to staff list
            }

            CheckingForFreeStaff(); // Checking for free staff
        }

        /// <summary>
        /// Looing for available idle staff point
        /// </summary>
        /// <returns> staff idle point </returns>
        private StaffPoint GetAvailableStaffPoint()
        {
            var staffPoint = _staffPointsList.Find(x => x.isAvailable); // get available staff point

            if (staffPoint == null) // (Error handler) if there is no more staff point
            {
                Debug.LogWarning("Add More Staff Points");
                return new StaffPoint() { PositionPoints = Vector2.zero, isAvailable = true };
            }

            return staffPoint; // return available staff point
        }

        /// <summary>
        /// Staff idle position
        /// </summary>
        public class StaffPoint
        {
            /// <summary>
            /// Staff idle point
            /// </summary>
            public Vector2 PositionPoints;

            /// <summary>
            /// Available state
            /// true if not booked yet
            /// false if there is staff booked
            /// </summary>
            public bool isAvailable;
        }
    }
}