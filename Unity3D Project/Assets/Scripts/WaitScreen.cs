using UnityEngine;
using System.Collections;

public class WaitScreen : MonoBehaviour {
	#region singletonImplementation
	static WaitScreen instance = null;
	public static WaitScreen Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<WaitScreen>();
			}
			return instance;
		}
	}
	WaitScreen() {
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null) {
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original WaitScreen instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	[SerializeField]
	UnityEngine.UI.Text mText;

	public void ShowWaitScreen(string pText = "Please wait")
	{
		gameObject.SetActive(true);
		mText.text = pText;
	}
	public void HideWaitScreen() {
		gameObject.SetActive(false);
	}
}
