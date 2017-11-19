using UnityEngine;
using System.Collections;

public abstract class PopUp : MonoBehaviour {
	public void ActivatePopUp()
	{
		gameObject.SetActive(true);
	}
	public void DeactivatePopUp()
	{
		gameObject.SetActive(false);
	}
}
