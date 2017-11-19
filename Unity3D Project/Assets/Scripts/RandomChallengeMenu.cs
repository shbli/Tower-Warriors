using UnityEngine;
using System.Collections;

public class RandomChallengeMenu : PopUp {
	#region singletonImplementation
	static RandomChallengeMenu instance = null;
	public static RandomChallengeMenu Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<RandomChallengeMenu>();
			}
			return instance;
		}
	}
	RandomChallengeMenu() {
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null) {
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original RandomChallengeMenu instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	public void OnExitClicked()
	{
		WaitScreen.Instance.ShowWaitScreen("Disconnecting");
		PhotonController.Instance.timeOut();
	}
}
