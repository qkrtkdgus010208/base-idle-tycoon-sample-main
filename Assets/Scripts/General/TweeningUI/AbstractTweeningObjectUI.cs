namespace Project
{
    using System;
    using UnityEngine;
	using DG.Tweening;

    public abstract class AbstractTweeningObjectUI : MonoBehaviour
	{
		/// <summary>
		/// Do dotween animation
		/// </summary>
		public abstract void DoAnimationUI();
	}
}
