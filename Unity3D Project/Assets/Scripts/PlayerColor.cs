using UnityEngine;
using System.Collections;

/// <summary>
/// Change a material depending on whether the player position (Player 1 or Player 2?)
/// </summary>
public class PlayerColor : MonoBehaviour
{
    [SerializeField]
    Material playerOneMat;
    [SerializeField]
    Material playerTwoMat;

	// Use this for initialization
	private void Start()
    {
        if (GetComponentInParent<NavTileAgent>().isLocalAgent)
        {
            GetComponent<Renderer>().sharedMaterial = playerOneMat;
        }
        else
        {
            GetComponent<Renderer>().sharedMaterial = playerTwoMat;
        }
	}
}
