using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoving
{
    void Register (int indexPoint);
    void Stop ();
    void UnRegister ();
    void EnableFx ();
    void DisableFx ();

    Transform GetTransform ();
}