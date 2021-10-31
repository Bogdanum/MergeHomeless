using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIdle
{
    void IdleRegister ();
    void IdleUnRegister ();
    void EarnCoins ();
    
    Vector3 GetPosition ();

    bool IsStop ();
}