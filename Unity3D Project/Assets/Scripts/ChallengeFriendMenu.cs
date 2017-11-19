using UnityEngine;
using System.Collections;

public class ChallengeFriendMenu : PopUp {
	#region singletonImplementation
	static ChallengeFriendMenu instance = null;
	public static ChallengeFriendMenu Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<ChallengeFriendMenu>();
			}
			return instance;
		}
	}
	ChallengeFriendMenu() {
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null) {
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original ChallengeFriendMenu instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	public void OnCancelClicked()
	{
		WaitScreen.Instance.ShowWaitScreen("Disconnecting");
		PhotonController.Instance.timeOut();
	}

	public void OnResendInviteClicked()
	{
		GameController.Instance.StartGame();
	}
}
