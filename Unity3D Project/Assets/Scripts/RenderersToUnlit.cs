using UnityEngine;
using System.Collections;

public class RenderersToUnlit : MonoBehaviour {
	// Use this for initialization
	void Start ()
	{
		Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < allRenderers.Length; i++) {
//			Debug.Log("Shader name is " + allRenderers [i].material.shader.name);
//			if (allRenderers [i].material.shader.name.Contains( "Diffuse" ))
//			{
				allRenderers [i].material.shader = Shader.Find("Unlit/Color");
				allRenderers [i].material.color = Color.black;
//			}
		}
	}
}
