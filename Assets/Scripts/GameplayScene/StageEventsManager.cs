namespace Project.Gameplay
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public static class StageEventsManager
    {
        /// <summary>
        /// Event to find available kitchen table from kitchen station
        /// Parameter 1: string target kitchen station id
        /// Parameter 2: Callbacks with kitchen table idx in that kitchen station, table point position, table position x
        /// </summary>
        public static Action<string, Action<int, Vector2, float>> GetKitchenTablePathPoint;

        /// <summary>
        /// Event to operate kitchen table in kitchen station
        /// Parameter 1: string target kitchen station id
        /// Parameter 2: int kitchen table idx in that kitchen station
        /// Parameter 3: After operate callbacks with long order profit
        /// </summary>
        public static Action<string, int, Action<long>> OperateKitchenTable;

        /// <summary>
        /// Event to get order table by customer idx
        /// Parameter 1: int customer idx
        /// Parameter 2: After get the table callback with table point position, table position x
        /// </summary>
        public static Action<int, Action<Vector2, float>> GetOrderTableByCustomerIdx;

        /// <summary>
        /// Event to get available order table
        /// Parameter 1: Order data that customer will order
        /// Parameter 2: After get the order table callback with table point position, table position x
        /// </summary>
        public static Action<List<OrderData>, Action<Vector2, float>, Action> GetOrderTablePathPoint;

        /// <summary>
        /// Customer event to request order
        /// Parameter 1: int customer idx that request to order
        /// </summary>
        public static Action<int> RequestToOrder;

        /// <summary>
        /// Staff event to list customer order
        /// Parameter 1: int customer idx that order
        /// </summary>
        public static Action<int> ListOrder;

        /// <summary>
        /// Event serve order to customer
        /// Parameter 1: Customer order data
        /// </summary>
        public static Action<OrderData> OrderToServe;

        /// <summary>
        /// Event to check available staff to do task
        /// </summary>
        public static Action CheckingFreeStaff;

        /// <summary>
        /// Event to get available task
        /// Parameter 1: available staff that will receive task if any available task
        /// </summary>
        public static Action<StaffController> GetTaskToDo;

        /// <summary>
        /// Event to increase staff slot
        /// Parameter 1: staff id that will spawn
        /// Parameter 2: int slot count to add
        /// </summary>
        public static Action<StaffController.Staff_ID, int> OnIncreaseStaffSlot;

        /// <summary>
        /// Event to increase customer slot
        /// Parameter 1: int slot count to add
        /// </summary>
        public static Action<int> OnIncreaseCustomerSlot;

        /// <summary>
        /// Event to change dress
        /// Parameter 1: Dress object data
        /// </summary>
        public static Action<DressObject> OnChangeDress;

        /// <summary>
        /// Event to add general profit bonus
        /// Parameter 1: float general profit bonus value
        /// </summary>
        public static Action<float> AddGeneralProfitBonus;

        /// <summary>
        /// Event to increase movement bonus
        /// Parameter 1: staff id that affected
        /// Parameter 2: float increase movement bonus value
        /// </summary>
        public static Action<StaffController.Staff_ID, float> AddMovementBonus;

        /// <summary>
        /// Event to increase movement bonus
        /// Parameter 1: string kitchen station id that profit bonus increase
        /// Parameter 2: float increase profit bonus value
        /// </summary>
        public static Action<string, float> AddKitchenProfitBonus;

        /// <summary>
        /// Event to increase reduce time
        /// Parameter 1: string kitchen station id that reduce time increase
        /// Parameter 2: float increase reduce time value
        /// </summary>
        public static Action<string, float> AddKitchenReduceTime;

        /// <summary>
        /// Event to reduce general profit bonus
        /// Parameter 1: float reduce profit bonus value
        /// </summary>
        public static Action<float> RemoveGeneralProfitBonus;

        /// <summary>
        /// Event to reduce movement bonus
        /// Parameter 1: staff id that affected
        /// Parameter 2: float reduce movement bonus value
        /// </summary>
        public static Action<StaffController.Staff_ID, float> RemoveMovementBonus;

        /// <summary>
        /// Event to update all kitchen station when profit bonus changed
        /// Parameter 1: float current profit bonus value
        /// </summary>
        public static Action<float> OnGeneralProfitBonusChanged;

        /// <summary>
        /// Event to update staff controller movement bonus
        /// Parameter 1: staff id that affected
        /// Parameter 2: float current movement bonus value
        /// </summary>
        public static Action<StaffController.Staff_ID, float> OnMovementBonusChanged;

        /// <summary>
        /// Event to update kitchen station when profit bonus changed
        /// Parameter 1: string kitchen station id that profit bonus changed
        /// Parameter 2: float current profit bonus
        /// </summary>
        public static Action<string, float> OnKitchenProfitBonusChanged;

        /// <summary>
        /// Event to update kitchen station when reduce time changed
        /// Parameter 1: string kitchen station id that reduce time changed
        /// Parameter 2: float current reduce time
        /// </summary>
        public static Action<string, float> OnKitchenReduceTimeChanged;

        /// <summary>
        /// Event to set kitchen station level
        /// Parameter 1: string kitchen station id
        /// Parameter 2: kitchen current level data
        /// Parameter 3: kitchen next level data
        /// </summary>
        public static Action<string, KitchenLevelData, KitchenLevelData> SetKitchenLevel;

        /// <summary>
        /// Event to scan path map
        /// </summary>
        public static Action ScanPathMap;

        public static Func<bool> IsAnyAvailableOrderTable;
        public static Func<Dictionary<SO_DishData, int>> GetRandomDishOrder;

        public static Func<float> GetCurrentGeneralProfitBonus;
        public static Func<StaffController.Staff_ID, float> GetCurrentMovementBonus;
        public static Func<string, float> GetKitchenProfitBonus;
        public static Func<string, float> GetKitchenReduceTime;
    }
}