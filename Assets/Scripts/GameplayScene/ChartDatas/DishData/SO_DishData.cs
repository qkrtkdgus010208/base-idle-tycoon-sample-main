namespace Project.Gameplay
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "new dish data", menuName = "Scriptable Objects/Dish Data")]
    public class SO_DishData : ScriptableObject
    {
        /// <summary>
        /// dish id
        /// </summary>
        [SerializeField] private string _dishID;

        /// <summary>
        /// dish id
        /// </summary>
        public string DishID => _dishID;

        /// <summary>
        /// dish name
        /// </summary>
        [SerializeField] private string _dishName;

        /// <summary>
        /// dish name
        /// </summary>
        public string DishName => _dishName;

        /// <summary>
        /// dish icon
        /// </summary>
        [SerializeField] private Sprite _dishIcon;

        /// <summary>
        /// dish icon
        /// </summary>
        public Sprite DishIcon => _dishIcon;

        /// <summary>
        /// dish process time
        /// </summary>
        [SerializeField] private float _dishProcessTime;

        /// <summary>
        /// dish process time
        /// </summary>
        public float DishProcessTime => _dishProcessTime;
    }
}