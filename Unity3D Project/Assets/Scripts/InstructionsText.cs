using UnityEngine;
using System.Collections;

public class InstructionsText : MonoBehaviour {
	#region singletonImplementation
	static InstructionsText instance = null;
	public static InstructionsText Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<InstructionsText>();
			}
			return instance;
		}
	}
	InstructionsText() {
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null) {
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original InstructionsText instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	[SerializeField]
	UnityEngine.UI.Text mText;

	// Use this for initialization
	private void Start ()
	{
		const float START_GAME_DELAY = 3f;
		StartCoroutine(StartGame(START_GAME_DELAY));
	}

	private IEnumerator StartGame(float delay)
	{
		yield return new WaitForSeconds(delay);
		BlackLoadingScreen.Instance.Fadeout();
	}

	public void onInstructionsTextClicked()
	{
		HideInstructionsText();
		MainMenu.Instance.ActivatePopUp();
	}

	public void ShowInstructionsText(string pText, bool clickable = false)
	{
		gameObject.SetActive(true);
		mText.text = pText;
		GetComponent<UnityEngine.UI.Button>().enabled = clickable;
	}
	public void HideInstructionsText() {
		gameObject.SetActive(false);
	}
}
