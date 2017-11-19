using UnityEngine;
using System.Collections;

public abstract class StateBase : MonoBehaviour {
	public abstract void startState ();
	public abstract void excuteState ();
	public abstract void endState ();
}
