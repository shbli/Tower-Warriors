using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	#region singletonImplementation
	static CameraController instance = null;
	public static CameraController Instance
	{
		get {
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<CameraController>();
			}
			return instance;
		}
	}
	CameraController()
	{
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null)
		{
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original CameraController instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	int selectedPosIndex = 0;
	[SerializeField] Transform portraitPositions;
	[SerializeField] Transform landscapePositions;
	[SerializeField] GameObject cameraButton;
	Transform selectedTransform;
	float dampTime = 0.15f;
	Vector3 velocity = Vector3.zero;
	int currentScreenW;
	int currentScreenH;
	// Use this for initialization
	void Start () {
		if (portraitPositions.childCount != landscapePositions.childCount)
		{
			Debug.LogError("portraitPositions.childCount != landscapePositions.childCount");
		}

		foreach(Transform _child in portraitPositions)
		{
			Destroy( _child.GetComponent<Camera>() );
		}

		foreach(Transform _child in landscapePositions)
		{
			Destroy( _child.GetComponent<Camera>() );
		}

		currentScreenW = Screen.width;
		currentScreenH = Screen.height;
	}

	public void ShowCameraButton()
	{
		cameraButton.gameObject.SetActive(true);
	}

	public void HideCameraButton()
	{
		cameraButton.gameObject.SetActive(false);
	}

	public void SetPlayerOneDefaultPos()
	{
		selectedPosIndex = 0;
		RePositionCamera();
	}

	public void SetPlayerTwoDefaultPos()
	{
		selectedPosIndex = 1;
		RePositionCamera();
	}

	public void NextCamera()
	{
		selectedPosIndex = ( ( selectedPosIndex + 1 ) % portraitPositions.childCount );
		RePositionCamera();
	}

	void RePositionCamera()
	{
		if (Screen.width > Screen.height)
		{
			//we are in a landscape screen
			selectedTransform = landscapePositions.GetChild(selectedPosIndex);
		}
		else
		{
			//we are in a portrait screen
			selectedTransform = portraitPositions.GetChild(selectedPosIndex);
		}

		iTween.Stop(gameObject);
		iTween.MoveTo(gameObject,
			iTween.Hash(
				iTween.HashKeys.time,0.75f,
				iTween.HashKeys.position,selectedTransform.position,
				iTween.HashKeys.islocal,false,
				iTween.HashKeys.easing,iTween.EaseType.easeInOutQuad
			)
		);
		iTween.RotateTo(gameObject,
			iTween.Hash(
				iTween.HashKeys.time,0.75f,
				iTween.HashKeys.rotation,selectedTransform.eulerAngles,
				iTween.HashKeys.islocal,false,
				iTween.HashKeys.easing,iTween.EaseType.easeInOutQuad
			)
		);
	}

    public void ShakeCamera() {
        if (GetComponent<iTween>() == null)
        {
            iTween.ShakeRotation(
                gameObject,
                iTween.Hash(
                    iTween.HashKeys.name,"cmrashk",
                    iTween.HashKeys.amount,Vector3.one*4,
                    iTween.HashKeys.time,0.75f
                )
            );
        }
    }

	// Update is called once per frame
	void Update () {
		//constantly check if screen res changed (Device rotated ... etc)
		if (Screen.width == currentScreenW && Screen.height == currentScreenH)
		{
			//screen haven't changed, let's not do anything about it
			return;
		}
		//screen changed, let's update our values
		currentScreenW = Screen.width;
		currentScreenH = Screen.height;
		RePositionCamera();
	}
}
