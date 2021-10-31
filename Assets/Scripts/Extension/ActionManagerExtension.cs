using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionManagerExtension  {

	public static ActionEventDispatcher RegisterActionEvent ( this MonoBehaviour mono, ActionEnums.ActionID actionID, System.Action <object> callback )
	{
		if (ReferenceEquals (ActionEventDispatcher.InstanceAwake(), null)) {

			LogGame.Error ("[Action Event] Action Event Dispatcher Is Null!");
			return null;
		}

		return 	ActionEventDispatcher.Instance.RegisterEvent (actionID, callback);;
	}

	public static ActionEventDispatcher RemoveActionEvent ( this MonoBehaviour mono, ActionEnums.ActionID actionID, System.Action <object> callback )
	{
		if (ReferenceEquals (ActionEventDispatcher.InstanceAwake(), null)) {

			LogGame.Error ("[Action Event] Action Event Dispatcher Is Null!");
			return null;
		}

		return 	ActionEventDispatcher.Instance.RemoveEvent (actionID, callback);;
	}

	public static ActionEventDispatcher PostActionEvent (this MonoBehaviour mono , ActionEnums.ActionID actionID ,object param = null)
	{
		if (ReferenceEquals (ActionEventDispatcher.InstanceAwake (), null)) {
			LogGame.Error ("[Action Event] Action Event Dispatcher Is Null!");
			return null;
		}

		return ActionEventDispatcher.Instance.PostEvent (actionID, param);;
	}
}
