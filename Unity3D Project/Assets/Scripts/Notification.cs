using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Notification : MonoBehaviour
{
	#region singletonImplementation
	static Notification instance = null;
	public static Notification Instance
	{
		get {
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<Notification>();
			}
			return instance;
		}
	}
	Notification()
	{
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null)
		{
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original Notification instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	List <string> notifications = new List<string>();
	[SerializeField]
	UnityEngine.UI.Text notificationText;
	bool isNotficationOnScreen = false;

	public void ShowNotification(string pText)
	{
		if (!notifications.Contains(pText) && notificationText.text != pText)
		{
			notifications.Add(pText);
		}
		transform.parent.gameObject.SetActive(true);
		DisplayNextQuedNotfication();
	}

	void DisplayNextQuedNotfication()
	{
		if (isNotficationOnScreen) {
			return;
		}
		if (notifications.Count <= 0)
		{
			transform.parent.gameObject.SetActive(false);
			return;
		}

		notificationText.text = notifications[0];
		notifications.RemoveAt(0);
		isNotficationOnScreen = true;
		StartCoroutine(ShowNotification());
	}

	private IEnumerator ShowNotification()
	{
		yield return new WaitForEndOfFrame();
		GetComponent<Animator>().Play("DisplayNotification");
	}

	public void OnOneNotificationCompleted()
	{
		isNotficationOnScreen = false;
		notificationText.text = "";
		StartCoroutine(DelayDisplayNextQuedNotfication());
	}

	private IEnumerator DelayDisplayNextQuedNotfication()
	{
		const float CALL_DELAY = 1f;
		GetComponent<Animator>().Play("Empty");
		yield return new WaitForSeconds(CALL_DELAY);
		DisplayNextQuedNotfication();
	}
}
