namespace Project.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;


    public class StaffController : MonoBehaviour
    {
        public enum Staff_ID
        {
            Manager,
            StaffHelper
        }

        /// <summary>
        /// self staff id
        /// </summary>
        [SerializeField] private Staff_ID _staffID;

        /// <summary>
        /// path finding movement controller
        /// </summary>
        [SerializeField] private MoveController _moveController;

        /// <summary>
        /// Character sprite offset from movement point
        /// </summary>
        [SerializeField] private Vector2 _characterSpriteOffset;

        /// <summary>
        /// Character sprite animator controller
        /// </summary>
        [SerializeField] private CharacterAnimation _animator;

        /// <summary>
        /// Default movement speed
        /// </summary>
        [SerializeField] private float _movementSpeed;

        /// <summary>
        /// Waiting customer to order indicator
        /// </summary>
        [SerializeField] private GameObject _awaitUI;

        /// <summary>
        /// Waiting progression bar
        /// </summary>
        [SerializeField] private Image _fillAwaitProgressUI;

        /// <summary>
        /// Current order dish icon
        /// </summary>
        [SerializeField] private Image _dishOnHold;

        /// <summary>
        /// Current order profit text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _profitText;

        /// <summary>
        /// Staff idle point position
        /// </summary>
        private StaffManager.StaffPoint _staffPoint;

        /// <summary>
        /// Staff bussy state
        /// true if staff doing task
        /// false if staff idle
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// Staff bussy state
        /// true if staff doing task
        /// false if staff idle
        /// </summary>
        public bool IsBusy => isBusy;

        /// <summary>
        /// Current kitchen table idx to operate
        /// </summary>
        private int currentKitchenTableIdx;

        /// <summary>
        /// Current order data to serve
        /// </summary>
        private OrderData currentOrder;


        private void Awake()
        {
            StageEventsManager.OnMovementBonusChanged += SetMovementBonus; // subs OnMovementBonusChanged events
        }

        private void OnDestroy()
        {
            StageEventsManager.OnMovementBonusChanged -= SetMovementBonus; // unsubs OnMovementBonusChanged events
        }

        private void Start()
        {
            _moveController.SetCharacterSprite( // set movement controler charater sprite
                _characterSpriteOffset,
                (state) => _animator.FlipCharacterSpriteState = state
                );

            SetMovementBonus(_staffID, StageEventsManager.GetCurrentMovementBonus(_staffID)); // set movement bonus
        }

        /// <summary>
        /// Set staff idle position point
        /// </summary>
        /// <param name="staffPoint"></param>
        public void SetStaffPoint(StaffManager.StaffPoint staffPoint)
        {
            staffPoint.isAvailable = false; // booked available staff point, make it unavailable
            _staffPoint = staffPoint; // Assign _staffPoint
        }

        /// <summary>
        /// Set movement bonus
        /// </summary>
        /// <param name="staffID"> staff id who get movement bonus </param>
        /// <param name="currentBonus"> float movement bonus value </param>
        private void SetMovementBonus(Staff_ID staffID, float bonusValue)
        {
            if (_staffID != staffID) return; // return if staff id not match
            _moveController.SetMovementSpeed(_movementSpeed * (bonusValue + 1)); // set movement bonus to movement controller
        }

        /// <summary>
        /// Staff receive task to serve order
        /// </summary>
        /// <param name="orderData"> order data to serve </param>
        public void ReceiveOrder(OrderData orderData)
        {
            isBusy = true; // set unavailabale for incoming task
            currentOrder = orderData; // Assign currentOrder order data

            StageEventsManager.GetKitchenTablePathPoint?.Invoke(currentOrder.DishData.DishID, OnGetKitchenTablePosition); // Get available kitchen table to operate
        }

        /// <summary>
        /// After get available kitchen table position
        /// </summary>
        /// <param name="kitchenTableIdx"> Available kitchen table idx in current order kitchen station </param>
        /// <param name="pathPoint"> Kitchen table stop point for path finding </param>
        /// <param name="tablePositionX"> Kitchen table x position for sprite flip state </param>
        private void OnGetKitchenTablePosition(int kitchenTableIdx, Vector2 pathPoint, float tablePositionX)
        {
            currentKitchenTableIdx = kitchenTableIdx; // Assign currentKitchenTableIdx to operate

            _animator.SetAnimationState(CharacterAnimation.Animation_State.Walking); // Set to walking animation
            _moveController.SetDestination(pathPoint, () => OnReachKitchenTable(tablePositionX)); // Set and move to current destination
        }

        /// <summary>
        /// After reach to kitchen table to operate
        /// </summary>
        /// <param name="tablePositionX"> Kitchen table x position for sprite flip state </param>
        private void OnReachKitchenTable(float tablePositionX)
        {
            _animator.FlipCharacterSpriteState = transform.position.x < tablePositionX; // Set flip sprite state
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Cooking); // Set to cooking animation

            StageEventsManager.OperateKitchenTable?.Invoke( // Start to operate kitchen table
                currentOrder.DishData.DishID, // Kitchen station id that handle current order dish id
                currentKitchenTableIdx, // Kitchen table idx in that kitchen station
                FindOrderTable // Callback after finish operate kitchen table
                );
        }

        /// <summary>
        /// Find order table which customer order the currentOrder
        /// </summary>
        /// <param name="orderProfit"> current order profit from kitchen station </param>
        private void FindOrderTable(long orderProfit)
        {
            currentOrder.Profit = orderProfit; // set current order price

            _dishOnHold.sprite = currentOrder.DishData.DishIcon; // set on hold dish icon
            _dishOnHold.gameObject.SetActive(true); // set active on hold dish image

            _profitText.SetText(Utility.StaticCurrencyStringConverison.GetString(currentOrder.Profit)); // set profit text output
            _profitText.gameObject.SetActive(true); // set active profit text output

            StageEventsManager.GetOrderTableByCustomerIdx?.Invoke(currentOrder.CustomerIdx, OnGetOrderTablePosition); // find order table which order currentOrder
        }

        /// <summary>
        /// After found order table that order currentOrder
        /// </summary>
        /// <param name="pathPoint"> Order table stop point for path finding </param>
        /// <param name="tablePositionX"> Kitchen table x position for sprite flip state </param>
        private void OnGetOrderTablePosition(Vector2 pathPoint, float tablePositionX)
        {
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Walking); // set to walking animation
            _moveController.SetDestination(pathPoint, () => ServeOrderToCustomer(tablePositionX)); // set and move to current destination
        }

        /// <summary>
        /// After reach order table to serve currentOrder
        /// </summary>
        /// <param name="tablePositionX"> Kitchen table x position for sprite flip state </param>
        private void ServeOrderToCustomer(float tablePositionX)
        {
            _animator.FlipCharacterSpriteState = transform.position.x < tablePositionX; // Set flip sprite state
            StageEventsManager.OrderToServe?.Invoke(currentOrder); // Serve currentOrder

            _dishOnHold.gameObject.SetActive(false); // Hide dish on hold image
            _profitText.gameObject.SetActive(false); // Hide current profit text

            OnTaskDone(); // Set task done
        }

        /// <summary>
        /// Receive task to customer request order
        /// </summary>
        /// <param name="customerIdx"> customer idx that request to order </param>
        public void ReceiveRequestOrder(int customerIdx)
        {
            isBusy = true; // set unavailabale for incoming task

            StageEventsManager.GetOrderTableByCustomerIdx?.Invoke( // Find order table which the customer have customerIdx
                customerIdx,
                (pathPoint, tablePositionX) => OnGetRequestOrderTablePosition(pathPoint, customerIdx, tablePositionX)
                );
        }

        /// <summary>
        /// After get order table that request to order
        /// </summary>
        /// <param name="pathPoint"> Order table stop point for path finding </param>
        /// <param name="customerIdx"> Customer idx that request to order </param>
        /// <param name="tablePositionX"> Kitchen table x position for sprite flip state </param>
        private void OnGetRequestOrderTablePosition(Vector2 pathPoint, int customerIdx, float tablePositionX)
        {
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Walking); // set to walking animation
            _moveController.SetDestination(pathPoint, () => ListCustomerOrder(customerIdx, tablePositionX)); // set and move to current destination
        }

        /// <summary>
        /// After reach order table to list customer order
        /// </summary>
        /// <param name="customerIdx"> Order table stop point for path finding </param>
        /// <param name="tablePositionX"> Kitchen table x position for sprite flip state </param>
        private void ListCustomerOrder(int customerIdx, float tablePositionX)
        {
            _animator.FlipCharacterSpriteState = transform.position.x < tablePositionX; // Set flip sprite state
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Idle); // set to walking animation

            
            StartCoroutine(AwaitCustomerRevealOrder(customerIdx)); // Waiting to costumer reveal order
        }

        // Waiting to costumer reveal order
        private IEnumerator AwaitCustomerRevealOrder(int customerIdx)
        {
            _fillAwaitProgressUI.fillAmount = 0; // reset progression bar
            _awaitUI.SetActive(true); // set active waiting indicator

            while (_fillAwaitProgressUI.fillAmount < Utility.StaticConstantDictionary.CUSTOMER_ORDER_TIME) // Waiting for CUSTOMER_ORDER_TIME
            { 
                // Fill progression bar
                yield return new WaitForEndOfFrame(); // each frame
                _fillAwaitProgressUI.fillAmount += Time.deltaTime; // fill progression bar per Time.deltaTime
            }

            _awaitUI.SetActive(false); // hide waiting indicator
            StageEventsManager.ListOrder?.Invoke(customerIdx); // push ListOrder events
            OnTaskDone(); // set task done
        }

        // After finish some task
        private void OnTaskDone()
        {
            isBusy = false; // set to available for incoming task
            
            // walking to idle position (will override if staff receive some task)
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Walking); // Set to walking
            _moveController.SetDestination(_staffPoint.PositionPoints, OnIdle); // set and move to current destination

            StageEventsManager.CheckingFreeStaff?.Invoke(); // Checking for available staff & task list
        }

        // After reach idle position
        private void OnIdle()
        {
            _animator.FlipCharacterSpriteState = !_animator.FlipCharacterSpriteState; // reverse flip character (change character flip from walking & facing idle position)
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Idle); // set to idle animation
        }
    }
}