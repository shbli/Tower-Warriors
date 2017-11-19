using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	Transform resizeableBar;
	float barMaxXScale;
	Vector3 originalBarScale;
	public float barPercantage = 100f;
	// Use this for initialization
	void Start () {
		resizeableBar = transform.Find("ResizeableBar");
		barMaxXScale = resizeableBar.localScale.x;
		originalBarScale = transform.localScale;
	}

	[ContextMenu("lookAtCamera")]
	public void lookAtCamera() {
		transform.LookAt(GameController.Instance.transform);
	}

	public void setBarPercantage(float pBarPercantage) {
		barPercantage = Mathf.Clamp(pBarPercantage,0,100);
		refreshBarPercantage();
	}

	[ContextMenu("refreshBarPercantage")]
	public void refreshBarPercantage() {
		//make sure bar is enabled
		enableBar();

		float calculatedXScale = (barPercantage/100f)*barMaxXScale;

		iTween.Stop(resizeableBar.gameObject);
		iTween.ScaleTo(resizeableBar.gameObject,
			iTween.Hash(
				iTween.HashKeys.scale,new Vector3(calculatedXScale,resizeableBar.localScale.y,resizeableBar.localScale.z),
				iTween.HashKeys.time,0.2f,
				iTween.HashKeys.islocal,true)
		);
	}

	void enableBar() {
		gameObject.SetActive(true);
		if (transform.localScale != originalBarScale) {
			if (GetComponent<iTween>() == null) {
				//we are not doing any animation on the bar, let's scale it up
				iTween.ScaleToPunch(gameObject,originalBarScale,0.5f);
			}
		}
	}

	public void hideBar() {
		iTween.Stop(gameObject);
		iTween.ScaleTo(gameObject,Vector3.zero,0.4f);
		CancelInvoke("disableBar");
		Invoke("disableBar",0.5f);
	}

	void disableBar() {
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		lookAtCamera();
	}
}
