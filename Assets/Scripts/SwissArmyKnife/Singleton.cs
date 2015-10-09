using UnityEngine;
using System.Collections;

namespace SwissArmyKnife
{
	/// <summary>
	/// Singleton.
	/// </summary>
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T	instance;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static T 	Instance
		{
		    get
		    {
				if (instance == null)
		        {
					instance = FindObjectOfType(typeof(T)) as T;
					if (instance == null)
		            { return null; }
		        }
				return instance;
		    }
		} 

		private void Awake()
		{
			if (instance == null)
		    { instance = this as T; }
			else if (instance != this as T)
		    {
		        Destroy(gameObject);
		        return;
		    }
		    Instance.AwakeSingleton();
		}

		/// <summary>
		/// Awakes the singleton. Replaces Unity Awake method.
		/// </summary>
		public virtual void AwakeSingleton() {}
	}
}