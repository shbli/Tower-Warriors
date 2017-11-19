using UnityEngine;
using System.Collections;

public class IceCube : MonoBehaviour {
	// Use this for initialization
	void Start ()
    {
        GridObjectBase targetObject = GetComponentInParent<GridObjectBase>();
        Freeze freeze = targetObject.gameObject.AddComponent<Freeze>();
        freeze.iceCubeAnimator = GetComponentInChildren<Animator>();
        freeze.iceCubeRoot = this.gameObject;
	}
}
