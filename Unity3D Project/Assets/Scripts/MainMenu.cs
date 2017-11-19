using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : PopUp
{
	#region singletonImplementation
	static MainMenu instance = null;
	public static MainMenu Instance
	{
		get {
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<MainMenu>();
			}
			return instance;
		}
	}
	MainMenu()
	{
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null)
		{
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original MainMenu instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	public void OnChallengeFriendClicked()
	{
		DeactivatePopUp();
		WaitScreen.Instance.ShowWaitScreen("Connecting");
		ChallengeFriendMenu.Instance.ActivatePopUp();
		PhotonController.Instance.createRoomForInvite();
	}

	public void OnRandomChallengeClicked()
	{
		DeactivatePopUp();
		WaitScreen.Instance.ShowWaitScreen("Connecting");
		RandomChallengeMenu.Instance.ActivatePopUp();
		PhotonController.Instance.joinRandomGame();
	}
}
