namespace Project.Gameplay
{
    using System;
    using UnityEngine;


    public class InputListener : MonoBehaviour
    {
        /// <summary>
        /// Tag object that will recognized to receive input
        /// </summary>
        private string UI_OBJECT_TAG = "UI_Object";

        /// <summary>
        /// Main button idx
        /// </summary>
        private const int LEFT_MOUSE_BUTTON_IDX = 0;

        /// <summary>
        /// Last interact object 
        /// that player will stop to interact to, 
        /// after player click outside object area
        /// </summary>
        private IInteractToInputPlayer lastInteractObj;

        /// <summary>
        /// input listener interupt state
        /// </summary>
        private bool isInputOnInterupt;

        /// <summary>
        /// Events to set interupt player input
        /// true: interupt to listen player input
        /// false: back to listen player input
        /// </summary>
        public static Action<bool> SetInteruptInputListener;


        private void Awake()
        {
            SetInteruptInputListener += OnInteruptInputListenerStateChanged;
        }

        private void OnDestroy()
        {
            SetInteruptInputListener -= OnInteruptInputListenerStateChanged;
        }

        private void Update()
        {
            if (isInputOnInterupt)
                return;

            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON_IDX)) // when player click some object
                CheckingRaycast();
        }

        /// <summary>
        /// Checking player input
        /// </summary>
        private void CheckingRaycast()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (hit.collider.CompareTag(UI_OBJECT_TAG))
                    return;

                IInteractToInputPlayer interactObj = hit.transform.GetComponent<IInteractToInputPlayer>();
                
                if (interactObj != null)
                {
                    if (lastInteractObj != null)
                        lastInteractObj.OnStopInteract();

                    lastInteractObj = interactObj;
                    lastInteractObj.OnPlayerClick();
                    return;
                }
            }
            else if (lastInteractObj != null)
            {
                lastInteractObj.OnStopInteract();
                lastInteractObj = null;
            }
        }

        /// <summary>
        /// on listen input player interupt state changed
        /// </summary>
        /// <param name="state"> true: interupt to listen player input / false: back to listen player input </param>
        private void OnInteruptInputListenerStateChanged(bool state)
            => isInputOnInterupt = state;
    }
}