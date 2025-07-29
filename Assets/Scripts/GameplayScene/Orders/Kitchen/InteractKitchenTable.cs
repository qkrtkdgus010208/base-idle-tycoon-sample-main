namespace Project.Gameplay
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Player input listener in kitchen station
    /// </summary>
	public class InteractKitchenTable : MonoBehaviour, IInteractToInputPlayer
	{
        /// <summary>
        /// Events when player click interact area
        /// </summary>
        private Action OnPlayerInteract;

        /// <summary>
        /// Events when player stop interact
        /// </summary>
        private Action OnPlayerStopInteract;

        /// <summary>
        /// Initialize events on player interact
        /// </summary>
        /// <param name="onInteract"> when player interact events </param>
        /// <param name="onStopInteract"> when player stop interact events </param>
        public void SetPlayerInteractAction(Action onInteract, Action onStopInteract)
        {
            OnPlayerInteract = onInteract;
            OnPlayerStopInteract = onStopInteract;
        }

        /// <summary>
        /// Events when player click interact area
        /// </summary>
        public void OnPlayerClick()
        {
            OnPlayerInteract?.Invoke();
        }

        /// <summary>
        /// Events when player stop interact
        /// </summary>
        public void OnStopInteract()
        {
            OnPlayerStopInteract?.Invoke();
        }
    }
}
