using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;




/// <summary>
/// Event manager, a singleton that triggers game events, you may subscribe to the events by passing the eventType and a callback methode to be called once the event is triggered
/// </summary>
public class EventManager : MonoBehaviour {
    /// <summary>
    /// The event types
    /// </summary>
    public enum EventType
    {
        AllPlayersEndedDeploy,
        TurnEnded
    }
	#region singletonFields
	public static EventManager Instance { get { return instance; } }
	private static EventManager instance = null;
	#endregion
	#region privateFields
	private Dictionary <EventType, UnityEvent> eventDictionary = new Dictionary<EventType, UnityEvent>();
	#endregion

    // Use this for initialization
    private void Awake ()
    {
        if (instance != null)
        {
            Debug.LogError("More than one event manager in scene");
        }
        instance = this;
    }

	/// <summary>
	/// Starts listening.
	/// </summary>
	/// <param name="eventType">The event you are intrested to listen to.</param>
	/// <param name="listner">The callback methode that will be triggered once the event is raised.</param>
	public void StartListening(EventType eventType, UnityAction listner)
	{
		UnityEvent thisEvent = null;
		eventDictionary.TryGetValue(eventType,out thisEvent);
		//if we haven't created such an event before, we create a new one 
		if (thisEvent == null)
		{
			thisEvent = new UnityEvent();
			eventDictionary.Add(eventType,thisEvent);
		}
		thisEvent.AddListener(listner);
	}

	/// <summary>
	/// Stops listening.
	/// </summary>
	/// <param name="eventType">The event you are intrested to stop listening to.</param>
	/// <param name="listner">The callback methode.</param>
	public void StopListening(EventType eventType, UnityAction listner)
	{
		UnityEvent thisEvent = null;
		eventDictionary.TryGetValue(eventType,out thisEvent);
		if (thisEvent != null)
		{
			thisEvent.RemoveListener(listner);
		}
	}


	/// <summary>
	/// Triggers the event passed, ie Call all the subscribed callbacks to that event.
	/// </summary>
	/// <param name="eventType">Event type.</param>
	public void TriggerEvent(EventType eventType)
	{
		UnityEvent thisEvent = null;
		eventDictionary.TryGetValue(eventType,out thisEvent);
		if (thisEvent != null)
		{
			thisEvent.Invoke();
		}
	}
}


