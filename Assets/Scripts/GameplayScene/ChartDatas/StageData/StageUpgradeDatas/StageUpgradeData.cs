namespace Project.Gameplay
{
	using System;

	[Serializable]
	public class StageUpgradeData
	{
		/// <summary>
		/// Stage upgrade level
		/// </summary>
		public int Level;

		/// <summary>
		/// Stage upgrade title
		/// </summary>
		public string Title;

		/// <summary>
		/// Price for this upgrade
		/// </summary>
		public long Price;

		/// <summary>
		/// Effect description
		/// </summary>
		public string EffectDescription;

		/// <summary>
		/// Effect code for this upgrade
		/// </summary>
		public string EffectCode;
	}
}
