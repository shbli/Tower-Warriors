using UnityEngine;
using System.Collections;

public class DeployBar : MonoBehaviour {
	#region singletonImplementation
	static DeployBar instance = null;
	public static DeployBar Instance
	{
		get {
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<DeployBar>();
			}
			return instance;
		}
	}
	DeployBar()
	{
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null)
		{
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original DeployBar instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion
	// Use this for initialization
	void Start ()
	{
	
	}

	public void ShowDeployBar()
	{
		gameObject.SetActive(true);
		GetComponent<Animator>().Play("DisplayDeployBar");
	}
	public void HideDeployBar()
	{
		GetComponent<Animator>().Play("HideDeployBar");
	}

	public void OnDeployBarHideCompleted()
	{
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
