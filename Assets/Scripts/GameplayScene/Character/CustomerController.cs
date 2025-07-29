namespace Project.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;


    public class CustomerController : MonoBehaviour
    {
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
        /// despawn point target destination
        /// </summary>
        private Vector2 despawnPoint;

        /// <summary>
        /// Customer current orders
        /// </summary>
        private List<OrderData> orders = new List<OrderData>();

        /// <summary>
        /// Customer porfit bonus multiplier
        /// </summary>
        private float bonusProfitsMultiply = 1f;

        /// <summary>
        /// Set customer element
        /// </summary>
        /// <param name="customerObjectData"> customer elements object data </param>
        public void SetCustomerElement(SO_CustomerObjectData customerObjectData)
        {
            _moveController.SetCharacterSprite( // set movement controler charater sprite
                _characterSpriteOffset,
                (state) => _animator.FlipCharacterSpriteState = state
                );

            _animator.SetController(customerObjectData.CustomerAsset); // set character skin to animator
            customerObjectData.CustomerEffect.ActiveEffect(this); // activate customer effect if any
        }

        /// <summary>
        /// Spawn customer
        /// </summary>
        /// <param name="customerIdx"> customer idx </param>
        /// <param name="spawnPoint"> spawn position point </param>
        /// <param name="despawnPoint"> despawn position point </param>
        public void Spawn(int customerIdx, Vector2 spawnPoint, Vector2 despawnPoint)
        {
            orders.Clear(); // reset order

            var getDishOrder = StageEventsManager.GetRandomDishOrder(); // get random available dish
            foreach (var dish in getDishOrder.Keys) // foreach dish variant in getDishOrder
                for (int i = 0; i < getDishOrder[dish]; i++) // for count of dish ordered each dish variant
                    orders.Add(new OrderData() { // Adding to orders
                        DishData = dish, // assign dish data
                        CustomerIdx = customerIdx, // assign customer idx
                        CustomerBonusProfits = bonusProfitsMultiply // assign customer bonus profit
                    });

            this.despawnPoint = despawnPoint; // set despawn point
            transform.position = spawnPoint; // set position to spawn point
            gameObject.SetActive(true); // activate game object

            StageEventsManager.GetOrderTablePathPoint?.Invoke(orders, OnGetOrderTablePosition, OnOrderFinish); // find available order table
        }

        /// <summary>
        /// Get order table position to go
        /// </summary>
        /// <param name="stopPoint"> order table customer stop point for path finding </param>
        /// <param name="tablePositionX"> order table x position for sprite flip state </param>
        private void OnGetOrderTablePosition(Vector2 stopPoint, float tablePositionX)
        {
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Walking); // set to walking animation
            _moveController.SetDestination(stopPoint, () => OnReachOrderTable(tablePositionX)); // Moving to table stop point, on reach destination call OnReachOrderTable callback
        }

        /// <summary>
        /// After reach order table
        /// </summary>
        /// <param name="tablePositionX"> order table x position for sprite flip state </param>
        private void OnReachOrderTable(float tablePositionX)
        {
            _animator.FlipCharacterSpriteState = transform.position.x < tablePositionX; // set character sprite flip state
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Idle); // set to idle animation

            StageEventsManager.RequestToOrder?.Invoke(orders[0].CustomerIdx); // Push RequestToOrder event, send customr idx
            StageEventsManager.CheckingFreeStaff?.Invoke(); // Looking for free staff
        }

        /// <summary>
        /// After customer receive order
        /// </summary>
        private void OnOrderFinish()
        {
            _animator.SetAnimationState(CharacterAnimation.Animation_State.Walking); // set to walking animation
            _moveController.SetDestination(despawnPoint, Deactive); // Moving to despawn point, then deactive
        }

        /// <summary>
        /// Set customer profit bonus multiplier
        /// </summary>
        /// <param name="bonusMultiply"></param>
        public void SetProfitBonusMultiply(float bonusMultiply)
            => bonusProfitsMultiply = bonusMultiply; // Assign bonusProfitsMultiplys

        /// <summary>
        /// Deactive game object
        /// </summary>
        private void Deactive()
        {
            gameObject.SetActive(false); // Deactive game object
        }
    }
}