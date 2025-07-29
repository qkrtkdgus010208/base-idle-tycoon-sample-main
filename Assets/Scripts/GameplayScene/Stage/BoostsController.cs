namespace Project.Gameplay
{
	using System.Collections.Generic;
    using UnityEngine;


	public class BoostsController : MonoBehaviour
	{
		/// <summary>
		/// Profit bonus for all dish / kitchen
		/// </summary>
		private float _generalProfitBonus;

		/// <summary>
		/// Manager movement speed bonus
		/// </summary>
		private float _managerMovementBonus;

		/// <summary>
		/// staff helper movement speed bonus
		/// </summary>
		private float _staffHelperMovementBonus;

		/// <summary>
		/// Profit bonus for each kitchen
		/// </summary>
		private Dictionary<string, float> _kitchensProfitBonus = new Dictionary<string, float>();

		/// <summary>
		/// Each kitchen reduce time 
		/// </summary>
		private Dictionary<string, float> _kitchensReduceTime = new Dictionary<string, float>();


        private void Awake()
        {
			StageEventsManager.AddGeneralProfitBonus += AddGeneralProfitBonus;
			StageEventsManager.AddMovementBonus += AddMovementBonus;
			StageEventsManager.AddKitchenProfitBonus += AddKitchenProfitBonus;
			StageEventsManager.AddKitchenReduceTime += AddKitchenReduceTime;

			StageEventsManager.RemoveGeneralProfitBonus += RemoveGeneralProfitBonus;
			StageEventsManager.RemoveMovementBonus += RemoveMovementBonus;

			StageEventsManager.GetCurrentGeneralProfitBonus = GetGeneralProfitBonus;
			StageEventsManager.GetCurrentMovementBonus = GetMovementBonus;
			StageEventsManager.GetKitchenProfitBonus = GetKitchenProfitBonus;
			StageEventsManager.GetKitchenReduceTime = GetKitchenReduceTime;
		}

        private void OnDestroy()
        {
			StageEventsManager.AddGeneralProfitBonus -= AddGeneralProfitBonus;
			StageEventsManager.AddMovementBonus -= AddMovementBonus;
			StageEventsManager.AddKitchenProfitBonus -= AddKitchenProfitBonus;
			StageEventsManager.AddKitchenReduceTime -= AddKitchenReduceTime;

			StageEventsManager.RemoveGeneralProfitBonus -= RemoveGeneralProfitBonus;
			StageEventsManager.RemoveMovementBonus -= RemoveMovementBonus;

			StageEventsManager.GetCurrentGeneralProfitBonus = null;
			StageEventsManager.GetCurrentMovementBonus = null;
			StageEventsManager.GetKitchenProfitBonus = null;
			StageEventsManager.GetKitchenReduceTime = null;
		}

		/// <summary>
		/// Get General Profit Bonus
		/// </summary>
		/// <returns> General Profit Bonus </returns>
		private float GetGeneralProfitBonus()
			=> _generalProfitBonus;

		/// <summary>
		/// Get movement bonus by Staff_ID
		/// </summary>
		/// <param name="staffID"> Staff id target </param>
		/// <returns> movement bonus value </returns>
		private float GetMovementBonus(StaffController.Staff_ID staffID)
			=> staffID switch // Get movement bonus by Staff_ID
			{
				StaffController.Staff_ID.Manager => _managerMovementBonus, // staffID == Manager
				StaffController.Staff_ID.StaffHelper => _staffHelperMovementBonus, // staffID == Helper
				_ => 0 // Default value
			};

		/// <summary>
		/// Get Kitchen Reduce Time 
		/// </summary>
		/// <param name="id"> Kitchen station id target </param>
		/// <returns> Kitchen Profit Bonus </returns>
		private float GetKitchenProfitBonus(string id)
		{
			if (!_kitchensProfitBonus.ContainsKey(id)) // if kitchen don't have profit bonus
				return 0;

			return _kitchensProfitBonus[id];
		}

		/// <summary>
		/// Get Kitchen Reduce Time 
		/// </summary>
		/// <param name="id"> Kitchen station id target </param>
		/// <returns> Kitchen reduce time process </returns>
		private float GetKitchenReduceTime(string id)
		{
			if (!_kitchensReduceTime.ContainsKey(id)) // if kitchen don't have reduce time yet
				return 0;

			return _kitchensReduceTime[id];
		}

		/// <summary>
		/// Add General Profit Bonus
		/// </summary>
		/// <param name="value"> profit bonus value </param>
		private void AddGeneralProfitBonus(float value)
		{
			_generalProfitBonus += value; // increase general profit bonus
			StageEventsManager.OnGeneralProfitBonusChanged?.Invoke(_generalProfitBonus);  // Push OnGeneralProfitBonusChanged events
		}

		/// <summary>
		/// Add staff movement bonus
		/// </summary>
		/// <param name="staffID"> Staff id target </param>
		/// <param name="value"> Movement bonus value </param>
		private void AddMovementBonus(StaffController.Staff_ID staffID, float value)
		{
            switch (staffID) // staff id target
            {
				case StaffController.Staff_ID.Manager: // Manager
					_managerMovementBonus += value; // increase movement bonus
					StageEventsManager.OnMovementBonusChanged?.Invoke(staffID, _managerMovementBonus); // Push OnMovementBonusChanged event
					break;
				case StaffController.Staff_ID.StaffHelper: // Staff helper
					_staffHelperMovementBonus += value; // increase movement bonus
					StageEventsManager.OnMovementBonusChanged?.Invoke(staffID, _staffHelperMovementBonus); // Push OnMovementBonusChanged event
					break;
            }

		}

		/// <summary>
		/// Add kitchen profit bonus
		/// </summary>
		/// <param name="id"> kitchen station target </param>
		/// <param name="value"> profit bonus value </param>
		private void AddKitchenProfitBonus(string id, float value)
        {
			if (!_kitchensProfitBonus.ContainsKey(id)) // if kitchen don't have profit bonus yet
				_kitchensProfitBonus.Add(id, value); // add kithcen id then set profit bonus value
			else // if kitchen have profit bonus before
				_kitchensProfitBonus[id] += value; // increase profit bonus

			StageEventsManager.OnKitchenProfitBonusChanged?.Invoke(id, _kitchensProfitBonus[id]); // Push OnKitchenProfitBonusChanged event
		}

		/// <summary>
		/// Add kitchen reduce time
		/// </summary>
		/// <param name="id"> kitchen station target </param>
		/// <param name="value"> reduce time value </param>
		private void AddKitchenReduceTime(string id, float value)
		{
			if (!_kitchensReduceTime.ContainsKey(id)) // if kitchen don't have reduce time yet
				_kitchensReduceTime.Add(id, value); // add kithcen id then set reduce time value
			else // if kitchen have reduce time before
				_kitchensReduceTime[id] += value; // increase kitchen reduce time
			
			StageEventsManager.OnKitchenReduceTimeChanged?.Invoke(id, _kitchensReduceTime[id]); // Push OnKitchenReduceTimeChanged event
		}

		/// <summary>
		/// Remove general profit bonus
		/// </summary>
		/// <param name="value"></param>
		private void RemoveGeneralProfitBonus(float value)
		{
			_generalProfitBonus -= value; // Remove general profit bonus
			StageEventsManager.OnGeneralProfitBonusChanged?.Invoke(_generalProfitBonus); // Push OnGeneralProfitBonusChanged event
		}

		/// <summary>
		/// Remove staff movement bonus
		/// </summary>
		/// <param name="staffID"> Staff id target </param>
		/// <param name="value"> Removed value </param>
		private void RemoveMovementBonus(StaffController.Staff_ID staffID, float value)
		{
			switch (staffID) // staff id target
			{
				case StaffController.Staff_ID.Manager: // Manager
					_managerMovementBonus -= value; // Remove manager movement bonus
					StageEventsManager.OnMovementBonusChanged?.Invoke(staffID, _managerMovementBonus); // Push OnMovementBonusChanged event
					break;
				case StaffController.Staff_ID.StaffHelper: // Staff helper
					_staffHelperMovementBonus -= value; // Remove staff helper movement bonus
					StageEventsManager.OnMovementBonusChanged?.Invoke(staffID, _staffHelperMovementBonus); // Push OnMovementBonusChanged event
					break;
			}
		}
	}
}
