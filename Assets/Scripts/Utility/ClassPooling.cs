namespace Project.Utility
{
	using System;
	using System.Collections.Generic;


	public class ClassPooling<T> where T : IPoolingObjects
	{
		/// <summary>
		/// Instantiate obj template function
		/// </summary>
		private Func<T> InstantiateObj;

		/// <summary>
		/// Pool Object
		/// </summary>
		private List<T> objs;

		/// <summary>
		/// Initaite class, Instantiate function required
		/// </summary>
		/// <param name="instantiateObj"></param>
		public ClassPooling(Func<T> instantiateObj)
        {
			InstantiateObj = instantiateObj; // Assign instantiate function
			objs = new List<T>(); // Create new pool
        }

		/// <summary>
		/// Get obj from pool that unactive
		/// </summary>
		/// <returns></returns>
		public T GetFromPool()
        {
			foreach (var obj in objs) // looking for all obj in pool
				if (!obj.IsActive()) // Get obj that unactive
					return obj;

			// Is there is no obj unactive in pool
			var newObj = InstantiateObj(); // Instantiate new obj
			objs.Add(newObj); // Adding new obj to pool
			return newObj;
		}

		/// <summary>
		/// Deactive all obj in pool
		/// </summary>
		public void ClearActiveObj()
        {
			foreach (var obj in objs)
				obj.SetActive(false);
        }
	}

	public interface IPoolingObjects
    {
		bool IsActive(); // Get obj active state
		void SetActive(bool state); // Set obj active state
    }
}
