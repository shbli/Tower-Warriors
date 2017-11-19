using UnityEngine;
using System.Collections;

public class Freeze : MonoBehaviour {
    //the root will be destoryed after the effect by 3 seconds
    public GameObject iceCubeRoot;
    public Animator iceCubeAnimator;

    int freezeCounter = 2;
	// Use this for initialization
	private void Start ()
    {
        EventManager.Instance.StartListening(EventManager.EventType.TurnEnded,OnLocalPlayerTurn);
	}

    private void OnDestroy()
    {
        EventManager.Instance.StopListening(EventManager.EventType.TurnEnded,OnLocalPlayerTurn);
    }
	
    [ContextMenu("OnLocalPlayerTurn")]
    public void OnLocalPlayerTurn()
    {
        freezeCounter--;
        if (freezeCounter <= 0)
        {
            iceCubeAnimator.Play("Hide");
            Destroy(iceCubeRoot,3.0f);
            Destroy(this);
        }
    }
}
