namespace Project.Gameplay
{
    using UnityEngine;
	using UnityEngine.Animations;


	public class CharacterAnimation : MonoBehaviour
	{
		/// <summary>
		/// Animator state parameter id
		/// to change state to animator
		/// </summary>
		private const string ANIMATION_STATE_PARAMETER = "AnimationEnumState";
		public enum Animation_State
        {
			Idle = 0,
			Walking = 1,
			Cooking = 2
        }

		/// <summary>
		/// Character sprite renderer
		/// </summary>
		[SerializeField] private SpriteRenderer _sr;

		/// <summary>
		/// Character animator
		/// </summary>
		[SerializeField] private Animator _anim;

		/// <summary>
		/// Set animator controller (Based on skin varian)
		/// </summary>
		/// <param name="animatorController"></param>
		public void SetController(RuntimeAnimatorController animatorController)
			=> _anim.runtimeAnimatorController = animatorController; // Assign animator controller to current character skin

		/// <summary>
		/// Set animation state
		/// </summary>
		/// <param name="animState"></param>
		public void SetAnimationState(Animation_State animState)
			=> _anim.SetInteger(ANIMATION_STATE_PARAMETER, (int)animState); // Set current state to animator 

		/// <summary>
		/// Set character sprite flip state
		/// </summary>
		public bool FlipCharacterSpriteState
		{
			get => _sr.flipX;
			set => _sr.flipX = value;
		}
	}
}
