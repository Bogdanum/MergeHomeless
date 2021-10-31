using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T> : MonoBehaviour  where T : MonoBehaviour  {

	
	[Header ("Singleton" )]
	[SerializeField] private bool _IsPersistent;

	public static T Instance;

	public static T InstanceAwake()
	{
		if ( ReferenceEquals ( Instance , null )) {
			Instance = FindObjectOfType (typeof(T)) as T;
		}

		return Instance;
	}

	protected virtual void Awake()
	{		
		if (ReferenceEquals (Instance, null)) {

			Instance = this as T;

		} else if (Instance != this as T) {

			Destroy (gameObject);

			return;
		}

		if (_IsPersistent) {
			DontDestroyOnLoad (Instance);
		}

		LogGame.Log (typeof(T).ToString () + " Awake Completed!");
	}

	protected virtual void OnDestroy()
	{
		if ( _IsPersistent == false ){
			Instance = null;
		}
	}
}
