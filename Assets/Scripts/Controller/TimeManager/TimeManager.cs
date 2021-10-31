using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
	private System.Action HandleOnUpdateTime;

	private float _CurrentTime;
	
	public TimeManager RegisterTimeUpdate (System.Action action)
	{
		HandleOnUpdateTime += action;

		return this;
	}

	public TimeManager UnregisterTimeUpdate (System.Action action)
	{
		HandleOnUpdateTime -= action;

		return this;
	}

	private void Start ()
	{
		RegisterTimeUpdate (OnUpdateTimes);
	}

	private void OnUpdateTimes ()
	{
		_CurrentTime += Time.deltaTime;
	}

	private void Update ()
	{
		HandleOnUpdateTime ();
	}
}
