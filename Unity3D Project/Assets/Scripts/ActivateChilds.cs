using UnityEngine;
using System.Collections;

public class ActivateChilds : MonoBehaviour {

	// Use this for initialization
	void Start () {
		foreach(Transform _child in transform)
		{
			_child.gameObject.SetActive(true);
		}
		Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
