using System;
using System.Collections.Generic;

namespace Project.Gameplay
{
	public static class PlayerWallet
	{
		/// <summary>
		/// Events when some currency update
		/// Currency.ID: currency id that updated
		/// long: currency current amount
		/// </summary>
		public static Action<Currency.ID, long> OnCurrencyUpdate;

		/// <summary>
		/// List of currency in wallet
		/// </summary>
		private static List<Currency> _currencies = new List<Currency>();

		/// <summary>
		/// Reset currency data in wallet
		/// </summary>
		public static void ResetData()
        {
			_currencies.Clear();
        }

		/// <summary>
		/// Initialize new currency to wallet
		/// </summary>
		/// <param name="currency"></param>
		public static void Initialize(Currency currency) => _currencies.Add(currency);

		/// <summary>
		/// Get current amount of specific currency
		/// </summary>
		/// <param name="currencyID"> target currency id </param>
		/// <param name="stageID"> target stage id </param>
		/// <returns> currency amount </returns>
		public static long GetCurrentCurrency(Currency.ID currencyID, string stageID)
        {
            var currency = _currencies.Find(x =>
			{
				if (currencyID == Currency.ID.Coins)
					return x.CurrencyID == currencyID && x.StageID == stageID;
				else if (currencyID == Currency.ID.Gems)
					return x.CurrencyID == currencyID;
				else
					return false;
			});

            return currency == null ? 0 : currency.Amount;
		}

		/// <summary>
		/// Add new player stage coin
		/// Whenever player moving to next level stage
		/// Will reset player coin to new stage coin
		/// </summary>
		/// <param name="stageID"> stage id </param>
		/// <param name="coinAmount"> starter coin amount of the stage </param>
		public static void AddNewPlayerStageCoin(string stageID, long coinAmount)
        {
			_currencies.Add(new Currency() { 
				CurrencyID = Currency.ID.Coins, 
				StageID = stageID, 
				Amount = coinAmount 
			});

			if (coinAmount > 0)
				SaveData.UserDataManager.Instance.AddPlayerTotalCoin(coinAmount);
        }

		/// <summary>
		/// Player income
		/// </summary>
		/// <param name="transmission"> income data </param>
		/// <param name="isSaveImmediately"> save Data Immediately state </param>
		public static void PlayerIncome(CurrencyTransmission transmission, bool isSaveImmediately = false)
		{
			var currency = _currencies.Find(x => // get specific currency in wallet that have same currency id with current currency transmission id
			{
				if (transmission.CurrencyID == Currency.ID.Coins) // if currency to update is coin, need to get which stage that need to updated
					return x.CurrencyID == transmission.CurrencyID && x.StageID == transmission.StageID;
				else if (transmission.CurrencyID == Currency.ID.Gems)
					return x.CurrencyID == transmission.CurrencyID;
				else 
					return false;
			});

			if (currency == null) // if currency not found in wallet
			{
				currency = new Currency()
				{
					CurrencyID = transmission.CurrencyID,
					StageID = transmission.StageID
				};

				_currencies.Add(currency); // add new currency to wallet
			}

			currency.Amount += transmission.Amount;

			if (currency.CurrencyID == Currency.ID.Coins) {
				SaveData.UserStageDataManager.Instance.UpdatePlayerCoinInStage(currency, isSaveImmediately);
				SaveData.UserDataManager.Instance.AddPlayerTotalCoin(transmission.Amount);
			}
			else
				SaveData.UserDataManager.Instance.UpdatePlayerCurrencyData(currency, isSaveImmediately);

			OnCurrencyUpdate?.Invoke(currency.CurrencyID, currency.Amount);
		}

		/// <summary>
		/// Player payment
		/// </summary>
		/// <param name="transmission"> income data </param>
		/// <param name="isSaveImmediately"> save Data Immediately state </param>
		/// <returns> true: payment success / false: invalid payment </returns>
		public static bool PlayerPayment(CurrencyTransmission transmission, bool isSaveImmediately = false)
		{
			var currency = _currencies.Find(x => // get specific currency in wallet that have same currency id with current currency transmission id
			{
				if (transmission.CurrencyID == Currency.ID.Coins) // if currency to update is coin, need to get which stage that need to updated
					return x.CurrencyID == transmission.CurrencyID && x.StageID == transmission.StageID;
				else if (transmission.CurrencyID == Currency.ID.Gems)
					return x.CurrencyID == transmission.CurrencyID;
				else
					return false;
			});

			// Some of payment may have zero price
			// So need add new currency to wallet, if that is new currency for the wallet
			if (currency == null) // if currency not found in wallet
			{
				currency = new Currency()
				{
					CurrencyID = transmission.CurrencyID,
					StageID = transmission.StageID
				};

				_currencies.Add(currency); // add new currency to wallet
			}

			if (currency.Amount < transmission.Amount) // Invalid payment
				return false;

			currency.Amount -= transmission.Amount;

			if (currency.CurrencyID == Currency.ID.Coins)
				SaveData.UserStageDataManager.Instance.UpdatePlayerCoinInStage(currency, isSaveImmediately);
			else
				SaveData.UserDataManager.Instance.UpdatePlayerCurrencyData(currency, isSaveImmediately);

			OnCurrencyUpdate?.Invoke(currency.CurrencyID, currency.Amount);
			return true; // Payment success
		}
	}
}
