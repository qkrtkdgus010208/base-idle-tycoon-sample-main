namespace Project
{
    using UnityEngine;


	public class FloatingTextPool : MonoBehaviour
	{
		/// <summary>
		/// Singleton
		/// </summary>
        private static FloatingTextPool instance;

		/// <summary>
		/// Singleton
		/// </summary>
		public static FloatingTextPool Instance => instance;
       
        /// <summary>
        /// floating text template
        /// </summary>
        [SerializeField] private FloatingTextObj _floatingTextTemplate;

		/// <summary>
		/// floating text parent
		/// </summary>
		[SerializeField] private Transform floatingTextParentTransform;

		/// <summary>
		/// Floating text pool
		/// </summary>
		private Utility.ClassPooling<FloatingTextObj> floatingTextPool;


		private void Awake()
        {
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			
			floatingTextPool = new Utility.ClassPooling<FloatingTextObj>(
				() => Instantiate(_floatingTextTemplate, floatingTextParentTransform));

			DontDestroyOnLoad(gameObject); 
        }

		/// <summary>
		/// Spawn floating text
		/// </summary>
		/// <param name="txt"> text message </param>
		/// <param name="pos"> spawn position </param>
		/// <param name="posState"> position state (screen space / world space) </param>
		/// <param name="textState"> message state (normal / invalid) </param>
		public void ShowFloatingText(string txt, Vector2 pos, FloatingTextObj.Position_State posState, FloatingTextObj.Text_State textState)
        {
			pos = posState == FloatingTextObj.Position_State.World ? Camera.main.WorldToScreenPoint(pos) : pos;

			var floatingTxt = floatingTextPool.GetFromPool();
			floatingTxt.SetText(txt, pos, textState);
			floatingTxt.SetActive(true);
        }
	}
}
