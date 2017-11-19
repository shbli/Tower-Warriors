using UnityEngine;
using System.Collections;

public class Dead : MonoBehaviour {

	// Use this for initialization
	private void Start ()
    {
        //hide the health bar
        GetComponentInChildren<HealthBar>().hideBar();

        //save the original rotation
        Vector3 diePosition = transform.position + new Vector3(0.0f,-5.0f,0.0f);

        const float ANIMATION_TIME = 0.5f;

        //animate to requuired rotation
        iTween.MoveTo(
            gameObject,
            iTween.Hash(
                iTween.HashKeys.position, diePosition,
                iTween.HashKeys.islocal, false,
                iTween.HashKeys.time, ANIMATION_TIME,
                iTween.HashKeys.oncompletetarget, gameObject,
                iTween.HashKeys.oncomplete, "OnDieAnimationCompleted"
            )
        );
	}

    private void OnDieAnimationCompleted()
    {
        if (GetComponent<NavTileAgent>().isLocalAgent)
        {
            GameController.Instance.RemoveControlableAgent(GetComponent<NavTileAgent>());
        }
        Destroy(gameObject);
    }
}
