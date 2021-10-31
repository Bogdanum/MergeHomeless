using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeedUp
{

	void ISpeedUpRegister ();
	void ISpeedUpUnRegister ();
	void ISpeedUpExecute ();
	void ISpeedUpStop ();

}
