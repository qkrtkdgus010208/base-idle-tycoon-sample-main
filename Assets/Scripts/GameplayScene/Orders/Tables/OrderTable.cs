namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public class OrderTable : MonoBehaviour
    {
        /// <summary>
        /// Customer stop position point
        /// </summary>
        [SerializeField] private Transform _customerPathPoint;

        /// <summary>
        /// Customer stop position point
        /// </summary>
        public Vector2 CustomerPathPoint => _customerPathPoint.position;

        /// <summary>
        /// Staff stop position point
        /// </summary>
        [SerializeField] private Transform _staffPathPoint;

        /// <summary>
        /// Staff stop position point
        /// </summary>
        public Vector2 StaffPathPoint => _staffPathPoint.position;

        /// <summary>
        /// order ui
        /// ui order that will appears when customer in this table have order
        /// </summary>
        [SerializeField] private GameObject _orderUI;

        /// <summary>
        /// list of item in order ui
        /// list of order that will appears when customer in this table have order
        /// </summary>
        [SerializeField] private OrderTableItemListUI[] _orderItemList;

        /// <summary>
        /// order finish callback for current customer in this table 
        /// </summary>
        private Action OnOrderFinish;

        /// <summary>
        /// is table active state
        /// true: already active
        /// false: still locked
        /// </summary>
        private bool isTableActive;

        /// <summary>
        /// is table active state
        /// true: already active
        /// false: still locked
        /// </summary>
        public bool IsTableActive => isTableActive;

        /// <summary>
        /// is table busy state
        /// true: there is customer in this table
        /// false: available for new customer
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// is table busy state
        /// true: there is customer in this table
        /// false: available for new customer
        /// </summary>
        public bool IsBusy => isBusy;

        /// <summary>
        /// Current customer order list
        /// </summary>
        private List<OrderData> currentOrders;

        /// <summary>
        /// Current customer order list
        /// </summary>
        public List<OrderData> CurrentOrders => currentOrders;

        /// <summary>
        /// Current customer idx
        /// </summary>
        private int currentCustomerIdx;

        /// <summary>
        /// Current customer idx
        /// </summary>
        public int CurrentCustomerIdx => currentCustomerIdx;

        /// <summary>
        /// Set table state active / inactive
        /// </summary>
        /// <param name="state"> true: set table active, false set table inactive </param>
        public void SetActiveTable(bool state)
        {
            isTableActive = state;
        }

        /// <summary>
        /// Set current customer for this table
        /// </summary>
        /// <param name="orders"> order list that customer order </param>
        /// <param name="onOrderFinish"> callback after all of order finish to serve </param>
        public void BookedTable(List<OrderData> orders, Action onOrderFinish)
        {
            isBusy = true;

            currentOrders = orders;
            currentCustomerIdx = orders[0].CustomerIdx;
            OnOrderFinish = onOrderFinish;
        }

        /// <summary>
        /// Showing order ui
        /// </summary>
        public void ShowListOrderUI()
        {
            SetOrderUI();
        }

        /// <summary>
        /// Each time order from current customer order list finish to serve
        /// </summary>
        /// <param name="order"></param>
        public void TakeOrder(OrderData order)
        {
            CurrentOrders.Remove(
                CurrentOrders.Find(
                    data => string.Equals(data.DishData.DishID, order.DishData.DishID)
                )
            );

            StageManager.Instance.PlayerCoinIncome((long)(order.Profit * order.CustomerBonusProfits));

            if (CurrentOrders.Count > 0) // if there is more order to serve
                SetOrderUI();
            else // when all order in current customer order list have finish to serve
            {
                _orderUI.SetActive(false);

                foreach (var orderItem in _orderItemList)
                    orderItem.SetActive(false);

                OnOrderFinish?.Invoke();
                isBusy = false;
            }
        }

        /// <summary>
        /// Set order list ui
        /// based on customer order
        /// </summary>
        private void SetOrderUI()
        {
            int counterItems = 1;
            int variantCount = 0;
            for (int i = 0; i < CurrentOrders.Count - 1; i++)
            {
                if (CurrentOrders[i].DishData == CurrentOrders[i + 1].DishData)
                    counterItems++;
                else
                {
                    _orderItemList[variantCount].SetOrderUI(CurrentOrders[i].DishData.DishIcon, counterItems);
                    _orderItemList[variantCount].SetActive(true);

                    if (i == CurrentOrders.Count - 1)
                    {
                        _orderItemList[variantCount + 1].SetOrderUI(CurrentOrders[i + 1].DishData.DishIcon, 1);
                        _orderItemList[variantCount + 1].SetActive(true);
                    }

                    variantCount++;
                    counterItems = 1;
                }
            }

            _orderItemList[variantCount].SetOrderUI(CurrentOrders[CurrentOrders.Count - 1].DishData.DishIcon, counterItems);
            _orderItemList[variantCount].SetActive(true);

            _orderUI.SetActive(true);
        }
    }
}