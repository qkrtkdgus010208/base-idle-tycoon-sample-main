using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/IAP Data/IAP Data Collections")]
public class SOIAPCollections : ScriptableObject
{
	/// <summary>
	/// List of iap data
	/// </summary>
	[SerializeField] private List<SOIAP> _iapItemCollections;

	/// <summary>
	/// List of iap data
	/// </summary>
	public List<SOIAP> IAPItemCollections => _iapItemCollections;

	/// <summary>
	/// Get iap data by iap id
	/// </summary>
	/// <param name="id"> iap id target </param>
	/// <returns> iap data </returns>
	public SOIAP GetIAPItemByID(string id)
		=> _iapItemCollections.Find(x => string.Equals(x.InAppPurchaseID, id));
}
