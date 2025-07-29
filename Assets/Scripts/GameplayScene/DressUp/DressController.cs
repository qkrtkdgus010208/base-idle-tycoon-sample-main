namespace Project.Gameplay
{
    using UnityEngine;


	public class DressController : MonoBehaviour
	{
        /// <summary>
        /// Character animator
        /// </summary>
		[SerializeField] private Animator _characterAnim;

        /// <summary>
        /// current used dress
        /// </summary>
		private DressObject currentDress;


        private void Awake()
        {
			StageEventsManager.OnChangeDress += ChangeDress;
        }

        private void OnDestroy()
        {
            StageEventsManager.OnChangeDress -= ChangeDress;
        }

        private void Start()
        {
            ChangeDress(WardrobeManager.GetDressObjectByID(SaveData.UserDataManager.Instance.UsedDressID));
        }

        /// <summary>
        /// Change dress
        /// </summary>
        /// <param name="dressData"> dress data to used </param>
        private void ChangeDress(DressObject dressData)
        {
			if (currentDress != null)
				currentDress.RemoveDress();

			currentDress = dressData;
			currentDress.WearDress(_characterAnim);
        }
	}
}
