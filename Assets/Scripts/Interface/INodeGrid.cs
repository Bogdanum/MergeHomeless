using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeGrid
{
	void EnableIconItem (int level, string key);
	void DisableIconItem ();
	void EnableIconBack ();
	void DisableIconBack ();
	void EnableOutline (int instanceId ,int level);
	void DisableOutline ();
	void SetInfo (int instanceId, int level);
	void SetState (bool isActive);
	void EnableLevel (int level);
	void DisableLevel ();
	void Enable ();
	void Disable ();
}
