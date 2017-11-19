using UnityEngine;
using System.Collections;

public class BlackLoadingScreen : MonoBehaviour {
	#region singletonImplementation
	static BlackLoadingScreen instance = null;
	public static BlackLoadingScreen Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<BlackLoadingScreen>();
			}
			return instance;
		}
	}
	BlackLoadingScreen() {
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null) {
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original BlackLoadingScreen instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	public void Fadeout() {
		Invoke("deactivate",1f);
		GetComponent<Animator>().Play("Fadeout");
	}

	void deactivate() {
		gameObject.SetActive(false);
	}

	public void Fadein() {
		CancelInvoke("deactivate");
		gameObject.SetActive(true);
		GetComponent<Animator>().Play("Fadein");
	}
}
