using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEventDispatcher : Singleton<ActionEventDispatcher>
{
    private readonly Dictionary<string, System.Action<object>> _PoolEvent = new Dictionary<string, System.Action<object>> ();

    private event System.Action<object> paramOut;

    public ActionEventDispatcher RegisterEvent (ActionEnums.ActionID actionID, System.Action<object> action)
    {
        if (_PoolEvent.TryGetValue (ActionEnums.GetActionString (actionID), out paramOut))
        {
            paramOut += action;

            _PoolEvent[ActionEnums.GetActionString (actionID)] = paramOut;
        }
        else
        {
            paramOut += action;

            _PoolEvent.Add (ActionEnums.GetActionString (actionID), paramOut);
        }

        LogGame.Log (string.Format ("[Action Manager] Register the new event with ID: {0}",
                                    ActionEnums.GetActionString (actionID)));

        return this;
    }

    public ActionEventDispatcher RemoveEvent (ActionEnums.ActionID actionID, System.Action<object> action)
    {
        if (_PoolEvent.TryGetValue (ActionEnums.GetActionString (actionID), out paramOut))
        {
            paramOut -= action;

            _PoolEvent[ActionEnums.GetActionString (actionID)] = paramOut;

            LogGame.Log (string.Format ("[Action Manager] Remove the event with ID: {0}",
                                        ActionEnums.GetActionString (actionID)));
        }
        else
        {
            LogGame.Log (string.Format ("[Action Manager] Not Found the event with ID: {0}",
                                        ActionEnums.GetActionString (actionID)));
        }

        return this;
    }

    public ActionEventDispatcher PostEvent (ActionEnums.ActionID actionID, object param)
    {
        if (!_PoolEvent.TryGetValue (ActionEnums.GetActionString (actionID), out paramOut)) return this;
        
        if (ReferenceEquals (paramOut, null))
        {
            _PoolEvent.Remove (ActionEnums.GetActionString (actionID));
            return this;
        }

        paramOut (param);

        LogGame.Log (string.Format ("[Action Manager] Post the event with ID: {0}",
                                    ActionEnums.GetActionString (actionID)));
        return this;
    }

    public void Clear ()
    {
        _PoolEvent.Clear ();
    }
}