using UnityEngine;
using System.Collections;

public class StateDeploy : StateBase {
	Camera gameCamera;
	string selectedDeployableObject;
    int selectedDeployableObjectPrice;

	Vector2 touchStartPos;
	float touchStartTime;


	private void Awake()
	{
		gameCamera = GetComponent<Camera>();
	}

    public void OnDeployableObjectSelected(string deployableObject, int goldAmount)
	{
        selectedDeployableObject = deployableObject;
        selectedDeployableObjectPrice = goldAmount;
		GameController.Instance.ChangeGameState(this);
	}

	public override void startState()
	{
        InstructionsText.Instance.ShowInstructionsText("Touch cell to deploy\nSwipe up to cancel");
	}

	public override void excuteState()
	{
		//wait for the deploy bar to be inactive first
		if (DeployBar.Instance.gameObject.activeInHierarchy)
			return;
		if (Input.GetMouseButtonDown(0))
		{
			touchStartPos = Input.mousePosition;
			touchStartTime = Time.timeSinceLevelLoad;
		}
		if (Input.GetMouseButtonUp(0))
		{
			float timeDiffrence = Time.timeSinceLevelLoad - touchStartTime;
			//the time diffrence when we consider this a tap or swipe, otherwise do nothing
			const float TAP_SWIPE_MAX_TIME = 0.5f;
			//the max distance to consider this a tap
			const float MAX_TAP_DISTANCE = 3f;
			if (timeDiffrence <= TAP_SWIPE_MAX_TIME) {
				//if true, it's a tap
				if (Vector2.Distance(touchStartPos,Input.mousePosition) <= MAX_TAP_DISTANCE) {
					LayerMask gridsLayer = 1 << LayerMask.NameToLayer("GridCube");
					RaycastHit vHit = new RaycastHit();
					Ray vRay = gameCamera.ScreenPointToRay(Input.mousePosition);
					if(Physics.Raycast(vRay, out vHit, 1000, gridsLayer)) 
					{
						Debug.Log("OK " + vHit.collider.gameObject.name);
						if (LevelArray.currentLevelArray.IsOccupied(vHit.collider.transform.position.x,vHit.collider.transform.position.z))
						{
							Notification.Instance.ShowNotification("Selected cell is already occupied");
							return;
						}
						//by checking the z index, we allow players to deploy only on the first 1 lines, similar to how chest start
						int zIndex = LevelArray.currentLevelArray.GetZIndex(vHit.collider.transform.position.z);
						if (PhotonController.Instance.isMaster)
						{
							if (zIndex <= 7)
							{
								Notification.Instance.ShowNotification("Not allowed");
								return;
							}
						}
						else
						{
							if (zIndex >= 3)
							{
								Notification.Instance.ShowNotification("Not allowed");
								return;
							}
						}
                        GoldController.Instance.decreaseGoldBy(selectedDeployableObjectPrice);
						BattleGOs.Instance.DeployBattleGO(selectedDeployableObject,vHit.collider.transform.position);
						//we are going to deploy another BattleGO
                        GameController.Instance.ChangeGameState(GetComponent<StateDeployObjectSelection>());
					}
				} else {
					//it's a drag/swipe, the distance is too far
					const float MIN_SWIPE_UP_DISTANCE = 10f;
					if ((Input.mousePosition.y - touchStartPos.y) >= MAX_TAP_DISTANCE) {
						//success, let's cancel the current deploy
						//get ready to deploy another BattleGO
                        GameController.Instance.ChangeGameState(GetComponent<StateDeployObjectSelection>());
					}
				}
			}
		}
	}

	public override void endState()
	{
		InstructionsText.Instance.HideInstructionsText();
	}
}
