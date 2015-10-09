using UnityEngine;
using System.Collections;

namespace SwissArmyKnife
{
	/// <summary>
	/// Singleton that is not destroyed automatically when loading a new scene.
	/// </summary>
	public class SingletonPersistent<T> : MonoBehaviour where T : SingletonPersistent<T>
	{
		private static T	instance;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static T		Instance
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
			DontDestroyOnLoad(this.gameObject);
			if (instance == null)
			{ instance = this as T; }
			else
			{
				Destroy(gameObject);
				return;
			}
			instance.AwakeSingleton();
		}
		
		/// <summary>
		/// Awakes the singleton. Replaces Unity Awake method.
		/// </summary>
		public virtual void AwakeSingleton() {}
	}
}