namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;



    public class OrdersManager : MonoBehaviour
    {
        /// <summary>
        /// Message appears after player AFK
        /// To notice player how much income they obtained
        /// While they AFK in saving power screen
        /// </summary>
        private const string AFK_INCOME_MESSAGE = "Income while AFK: ";

        /// <summary>
        /// Stage loader data container
        /// </summary>
        [SerializeField] private SaveData.SO_StageLoadData _stageDataLoad;

        /// <summary>
        /// Stage default data container
        /// </summary>
        [SerializeField] private SO_StageDefaultDatasCollections _stageDefaultDataCollection;

        /// <summary>
        /// Kitchen data container
        /// </summary>
        [SerializeField] private SO_BatchKitchenLevelDataCollection _kitchensData;

        /// <summary>
        /// List of order table in current played stage
        /// </summary>
        private List<OrderTable> _orderTables;

        /// <summary>
        /// List of kitchen in current played stage
        /// </summary>
        private List<Kitchenware> _kitchens;

        /// <summary>
        /// List of customer request to order
        /// Int: customer idx that want to order
        /// </summary>
        private List<int> orderToReveal = new List<int>();

        /// <summary>
        /// List of order that should staff serve
        /// OrderData: the order that customer order
        ///            some customer may have more than one order, 
        ///            So two or more order data may have same customer idx
        /// </summary>
        private List<OrderData> orderList = new List<OrderData>();

        /// <summary>
        /// List of available dish that customer can order
        /// </summary>
        private List<SO_DishData> currentAvailableDish = new List<SO_DishData>();

        /// <summary>
        /// Temporary list of randomize order
        /// for each time customer appear and get order
        /// this orders list is what will they orders
        /// </summary>
        private Dictionary<SO_DishData, int> tempCurrentRandomOrder = new Dictionary<SO_DishData, int>();

        /// <summary>
        /// Time when player start to AFK
        /// </summary>
        private DateTime playerStartAFKTime;

        /// <summary>
        /// Customer spawn time rate in current played stage
        /// </summary>
        private float _customerSpawnTimeRate;

        /// <summary>
        /// Customer maximal number of order can they have in current played stage
        /// </summary>
        private int _customerMaxOrder;

        /// <summary>
        /// Customer maximal number of order variant can they have in current played stage
        /// </summary>
        private int _customerMaxOrderVariant;

        private void Awake()
        {
            ApplicationEvents.Instance.OnPlayerAfkStateChange += OnPlayerAFK;

            StageEventsManager.GetOrderTablePathPoint += GetOrderTablePoint;
            StageEventsManager.GetOrderTableByCustomerIdx += GetOrderTablePositionByCustomerIdx;

            StageEventsManager.RequestToOrder += AddOrderToReveal;

            StageEventsManager.ListOrder += ListOrderByTableCustomerIdx;
            StageEventsManager.GetTaskToDo += CheckingForTask;
            StageEventsManager.OrderToServe += GetOrderTableWhichTakeTheOrder;

            StageEventsManager.OnIncreaseCustomerSlot += AddMoreOrderTable;
            
            StageEventsManager.GetRandomDishOrder = RandomCustomerOrder;
            StageEventsManager.IsAnyAvailableOrderTable = IsOrderTableAvailable;
        }

        private void OnDestroy()
        {
            ApplicationEvents.Instance.OnPlayerAfkStateChange -= OnPlayerAFK;

            StageEventsManager.GetOrderTablePathPoint -= GetOrderTablePoint;
            StageEventsManager.GetOrderTableByCustomerIdx -= GetOrderTablePositionByCustomerIdx;

            StageEventsManager.RequestToOrder -= AddOrderToReveal;

            StageEventsManager.ListOrder -= ListOrderByTableCustomerIdx;
            StageEventsManager.GetTaskToDo -= CheckingForTask;
            StageEventsManager.OrderToServe -= GetOrderTableWhichTakeTheOrder;
            
            StageEventsManager.OnIncreaseCustomerSlot -= AddMoreOrderTable;

            StageEventsManager.GetRandomDishOrder = null;
            StageEventsManager.IsAnyAvailableOrderTable = null;
        }

        private void Start()
        {
            LoadingUIController.Instance.Loading();
            
            SetupOrderTable();
            SetupKitchen();

            var currentPlayedStage = _stageDefaultDataCollection.GetStageDataById(_stageDataLoad.CurrentStageID);

            _customerSpawnTimeRate = currentPlayedStage.CustomerSpawnTime;
            _customerMaxOrder = currentPlayedStage.MaxCustomerOrderCount;
            _customerMaxOrderVariant = currentPlayedStage.MaxCustomerOrderVariant;


            StageEventsManager.ScanPathMap?.Invoke();
            LoadingUIController.Instance.FinishLoading();
        }

        /// <summary>
        /// Initialize all order table in current played stage
        /// </summary>
        private void SetupOrderTable()
        {
            var data = _stageDataLoad.CurrentStageOrderTablesData; // get all of order table in current played stage
            int startTableCount = _stageDefaultDataCollection.GetStageDataById(data.StageID).StartingOrderTableCount;
            
            _orderTables = new List<OrderTable>();
            for (int i = 0; i < data.OrderTable.Length; i++)
            {
                _orderTables.Add(Instantiate(data.OrderTable[i]));
                _orderTables[i].SetActiveTable(i < startTableCount);
            }
        }

        /// <summary>
        /// Initailize all kitchen in current played stage
        /// </summary>
        private void SetupKitchen()
        {
            var stageData = _stageDataLoad.CurrentStageKitchensData; // get all of kitchen station in current played stage
            
            var stageSaveData = SaveData.UserStageDataManager.Instance.GetStageDataByID(stageData.StageID); // get save data for current played stage
            var kitchenSaveData = stageSaveData == null ? new List<SaveData.UserStageDataManager.UserStageObjectData.KitchenDataInStage>() : // get list of kitchen save data from stage save data
                stageSaveData.KitchenDatas == null ? new List<SaveData.UserStageDataManager.UserStageObjectData.KitchenDataInStage>() : stageSaveData.KitchenDatas;

            _kitchens = new List<Kitchenware>();

            for (int i = 0; i < stageData.Kitchens.Length; i++)
            {
                var kitchenData = _kitchensData.GetKitchenDataByDishID(stageData.Kitchens[i].DishID);
                StageManager.Instance.SetStageData(kitchenData);

                Kitchenware kitchen = Instantiate(stageData.Kitchens[i]);
                
                var userKitchenData = kitchenSaveData.Find(x => string.Equals(x.KitchenDataID, kitchen.DishID));
                kitchen.SetupKitchen(
                    userKitchenData == null ? 0 : userKitchenData.KitchenDataLevel, // set kitchen current level from save data, if save data null then this kitchen level is 0
                    i == 0 ? null : _kitchens[i - 1] // set kitchen station requirement to activate this kitchen station, if this kitchen is first idx in this stage then this kitchen have no kitchen requirement to activate
                    );

                _kitchens.Add(kitchen);
            }
        }

        /// <summary>
        /// Add customer request to order task
        /// </summary>
        /// <param name="customerIdx"> customer idx that have to order </param>
        private void AddOrderToReveal(int customerIdx)
        {
            orderToReveal.Add(customerIdx);
        }

        /// <summary>
        /// Checking is there is available order table for customer
        /// </summary>
        /// <returns> true: there is available table 
        /// false: all of table in the field is unavailable </returns>
        private bool IsOrderTableAvailable()
        {
            foreach (OrderTable table in _orderTables)
            {
                if (!table.IsTableActive) continue;
                if (!table.IsBusy) return true;
            }

            return false;
        }

        /// <summary>
        /// Checking for task to staff do
        /// </summary>
        /// <param name="staff"> staff that will do the task (if any task to do) </param>
        private void CheckingForTask(StaffController staff)
        {
            if (orderToReveal.Count > 0) // Checking if any order to reveal first
            {
                staff.ReceiveRequestOrder(orderToReveal[0]); // The staff get this task
                orderToReveal.RemoveAt(0); // Remove the task from list
            }
            else
            {
                foreach (OrderData order in orderList) // Get order that can do (which the kitchenware is available)
                {
                    Kitchenware kitchen = _kitchens.Find(kitchen => string.Equals(kitchen.DishID, order.DishData.DishID));

                    if (kitchen.IsKitchenAvailable)
                    {
                        staff.ReceiveOrder(order); // The staff get this task
                        orderList.Remove(order); // Remove the task from list

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get available order table for customer
        /// </summary>
        /// <param name="orders"> customer order list </param>
        /// <param name="onGetTableCallbacks"> callback after get available order table </param>
        /// <param name="onOrderFinish"> callback after all of customer order finish to serve </param>
        private void GetOrderTablePoint(List<OrderData> orders, Action<Vector2, float> onGetTableCallbacks, Action onOrderFinish)
        {
            OrderTable freeTable = _orderTables.Find(table => table.IsTableActive && !table.IsBusy);

            freeTable.BookedTable(orders, onOrderFinish); // Set table available state is false
            onGetTableCallbacks?.Invoke(freeTable.CustomerPathPoint, freeTable.transform.position.x); // Send table path point
        }

        /// <summary>
        /// Add customer order into the list of order to serve
        /// </summary>
        /// <param name="customerIdx"> target customer idx that have order </param>
        private void ListOrderByTableCustomerIdx(int customerIdx)
        {
            orderToReveal.Remove(customerIdx);

            OrderTable orderTable = _orderTables.Find(table => table.CurrentCustomerIdx == customerIdx);

            foreach (var orderData in orderTable.CurrentOrders)
                orderList.Add(orderData);

            orderTable.ShowListOrderUI();
        }

        /// <summary>
        /// Get order table by customer idx
        /// Send the table path point
        /// </summary>
        /// <param name="customerIdx"> target customer idx in the order table </param>
        /// <param name="onGetTableCallbacks"> callback after getting the table </param>
        private void GetOrderTablePositionByCustomerIdx(int customerIdx, Action<Vector2, float> onGetTableCallbacks)
        {
            OrderTable orderTable = _orderTables.Find(table => table.CurrentCustomerIdx == customerIdx);
            onGetTableCallbacks?.Invoke(orderTable.StaffPathPoint, orderTable.transform.position.x);
        }

        /// <summary>
        /// Get order table which the order is ready to serve by staff
        /// </summary>
        /// <param name="orderData"> order data that staff will serve </param>
        private void GetOrderTableWhichTakeTheOrder(OrderData orderData)
        {
            OrderTable orderTable = _orderTables.Find(table => table.CurrentCustomerIdx == orderData.CustomerIdx);
            orderTable.TakeOrder(orderData);
        }

        /// <summary>
        /// Randomize order list for customer
        /// </summary>
        /// <returns> order list that will customer order </returns>
        private Dictionary<SO_DishData, int> RandomCustomerOrder()
        {
            if (currentAvailableDish.Count < _kitchens.Count)
                foreach (var kitchen in _kitchens) // Checking for all kitchens in current played stage
                    if (!currentAvailableDish.Contains(kitchen.DishData) && kitchen.IsKitchenActive) // Chekcing if kitchen active
                        currentAvailableDish.Add(kitchen.DishData); // if kitchen active, then the dish that kitchen handle is available to order

            if (currentAvailableDish.Count <= 0) // if there is no kitchen active (new stage played)
                currentAvailableDish.Add(_kitchens[0].DishData); // default available dish is dish with first index in list

            tempCurrentRandomOrder.Clear(); // clear temporary
                                            
            int currentOrderCount = UnityEngine.Random.Range(1, _customerMaxOrder + 1); // randomize customer order count, between 1 ~ _customerMaxOrder
            int orderVariantCount = UnityEngine.Random.Range(1, _customerMaxOrderVariant + 1); // randomize customer variant count, between 1 ~ _customerMaxOrderVariant

            while (currentOrderCount > 0) // for much number of current order count
            {
                var dish = currentAvailableDish[UnityEngine.Random.Range(0, currentAvailableDish.Count)]; // randomize dish data to order
                if (!tempCurrentRandomOrder.ContainsKey(dish)) // if dish data is not in order list
                {
                    if (orderVariantCount > 0) // checking if still have more order variant
                    {
                        tempCurrentRandomOrder.Add(dish, 1); // add more dish variant into order list
                        orderVariantCount--;
                    }
                    else // if order variant already 0
                        continue;
                }
                else // if dish data already in order list
                    tempCurrentRandomOrder[dish]++; // add more this dish order count

                currentOrderCount--;
            }

            return tempCurrentRandomOrder;
        }

        /// <summary>
        /// Add more order table into the field
        /// </summary>
        /// <param name="count"> number to add </param>
        private void AddMoreOrderTable(int count)
        {
            foreach (OrderTable table in _orderTables)
            {
                if (table.IsTableActive) continue;

                table.SetActiveTable(true);

                count--;
                if (count <= 0) break;
            }
        }

        /// <summary>
        /// When player afk state changed
        /// </summary>
        /// <param name="state"> true: player start to AFK,
        /// false: player back from AFK </param>
        private void OnPlayerAFK(bool state)
        {
            if (state) // when start to afk
                playerStartAFKTime = BackndServer.BackndServerTime.GetServerTime(); // assign start afk time
            else // when back from afk 
            {
                var currentTime = BackndServer.BackndServerTime.GetServerTime(); // time when player back from afk
                var longAFKTime = (float)currentTime.Subtract(playerStartAFKTime).TotalSeconds; // total afk time in seconds

                int customerCount = Mathf.FloorToInt(longAFKTime / _customerSpawnTimeRate); // calculate customer count
                long totalIncome = 0;
                for (int i = 0; i < customerCount; i++) // calculate income while player afk
                {
                    var customerOrderList = RandomCustomerOrder();

                    foreach (var order in customerOrderList)
                        totalIncome += _kitchens.Find(x => string.Equals(x.DishID, order.Key.DishID)).CalculateCurrentProfit() * order.Value;
                }

                StageManager.Instance.PlayerCoinIncome(totalIncome);
                NoticeUIController.Instance.ShowFlashNotice(AFK_INCOME_MESSAGE + Utility.StaticCurrencyStringConverison.GetString(totalIncome));
            }
        }
    }
}