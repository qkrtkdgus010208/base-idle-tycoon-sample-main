namespace Project.Utility
{
    using System.Globalization;
    using UnityEngine;


	public static class StaticCurrencyStringConverison
	{
		private const string FLOAT_STRING_FORMAT = "0.00";
		private const string STR_NUMBER_CODE = "N0";
		private const int TEN = 10;

		private static CultureInfo _localizationCurrencyCultureInfo = new CultureInfo("en-US");

		/// <summary>
		/// Currencies Data
		/// </summary>
		private static CurrencyData[] currenciesDatas = new CurrencyData[]
		{
			new CurrencyData(){ MinNumber = 1000, Symbol = 'K' },
			new CurrencyData(){ MinNumber = 1000000, Symbol = 'M' },
			new CurrencyData(){ MinNumber = 1000000000, Symbol = 'B' },
		};

		/// <summary>
		/// Conver long number to make it easier to read
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetString(long value)
        {
			for (int i = 0; i < currenciesDatas.Length; i++) // lookup all currency data
            {
				if (currenciesDatas[i].MinNumber > value) // Looking for currency data that minimum number exceed long number
					if (i == 0) break; // If first index in currenciesDatas minimum number is exceed long number, Convert long number is unnecessary
					else return GenerateString(currenciesDatas[i - 1], value); // Convert long number based on currenciesDatas index before this currency data

				if (i == currenciesDatas.Length - 1) // Last index in currenciesDatas
					return GenerateString(currenciesDatas[i], value); // Convert long number based on last index in currenciesDatas
			}

			return value.ToString();
        }

		/// <summary>
		/// Generate string after convert long number
		/// </summary>
		/// <param name="currencyData"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string GenerateString(CurrencyData currencyData, long value)
		{
			float cut = value / currencyData.MinNumber;
			if (cut < TEN) // If string conversion under 10, then make it float number (3.15M)
			{
				cut += (value - cut * currencyData.MinNumber) / currencyData.MinNumber;
				return cut.ToString(FLOAT_STRING_FORMAT) + currencyData.Symbol;
			}
			else // If string conversion equals or more than 10, then make it int number (315M)
			{
				var textOutput = Mathf.FloorToInt(cut).ToString(STR_NUMBER_CODE, _localizationCurrencyCultureInfo);
				return textOutput + currencyData.Symbol;
			}
		}

		private struct CurrencyData
		{
			public long MinNumber; // Minimum number
			public char Symbol; // Number symbol
		}
	}
}
