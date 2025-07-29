namespace Project
{
    using UnityEngine;
    using TMPro;
    using Project.Utility;

    public class FloatingTextObj : MonoBehaviour, IPoolingObjects
	{
        public enum Text_State
        {
            Normal,
            Invalid
        }

        public enum Position_State
        {
            World,
            Screen
        }

        /// <summary>
        /// Maximum distance position after spawn
        /// Before deactive
        /// </summary>
        private const float MAX_POSITION_Y_DISTANCE = 40f;

        /// <summary>
        /// message text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _txt;

        /// <summary>
        /// Text color for normal state
        /// </summary>
        [SerializeField] private Color normalColor;

        /// <summary>
        /// Text color for invalid state
        /// </summary>
        [SerializeField] private Color invalidColor;

        /// <summary>
        /// Object pool activate state
        /// </summary>
		private bool isActive;

        /// <summary>
        /// Object pool activate state
        /// </summary>
        /// <returns> true: object active / false: object inactive </returns>
		public bool IsActive() => isActive;

        /// <summary>
        /// Starting spawn position
        /// </summary>
        private Vector2 startingSpawnPos;

        /// <summary>
        /// Final y position before deactive
        /// </summary>
        private float finalYPos;


        private void Update()
        {
            if (!isActive) return;

            transform.position += 20f * Time.deltaTime * Vector3.up;
            
            if (transform.position.y > finalYPos)
                SetActive(false);
        }

        /// <summary>
        /// Set floating text
        /// </summary>
        /// <param name="txt"> text message </param>
        /// <param name="screenPos"> start position </param>
        /// <param name="messageState"> message text state (normal / invalid) </param>
		public void SetText(string txt, Vector2 screenPos, Text_State messageState)
        {
			isActive = true;

            startingSpawnPos = screenPos;
            finalYPos = startingSpawnPos.y + MAX_POSITION_Y_DISTANCE;

            _txt.SetText(txt);
            transform.position = startingSpawnPos;

            _txt.color = messageState switch
            {
                Text_State.Invalid => invalidColor,
                _ => normalColor,
            };
        }

        /// <summary>
        /// Set active object pool
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        public void SetActive(bool state)
        {
			isActive = state;
			gameObject.SetActive(state);
        }
    }
}
