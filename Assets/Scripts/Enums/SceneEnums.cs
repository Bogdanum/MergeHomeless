using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnums {

	public enum SceneID
	{
		SceneEmpty,
		SceneDev,
		SceneInit,
		SceneLoading,
	}

	private static readonly string[] SceneString =
	{
		"Scene-Empty",
		"Scene-Dev",
		"Scene-Init",
		"Scene-Loading"
	};
	
	public static string GetSceneString (SceneID id)
	{
		return SceneString[(int) id];
	}
}
