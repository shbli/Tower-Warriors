using UnityEngine;
using System.Collections;

public class StatePlayerSelectObject : StateBase
{
    Camera gameCamera;
    Vector2 touchStartPos;
    float touchStartTime;
    private void Awake()
    {
        gameCamera = GetComponent<Camera>();
    }

    public override void startState()
    {
        InstructionsText.Instance.ShowInstructionsText("Select object!");
        //allow player to end his turn at this moment
        GameController.Instance.EndTurnVisiable = true;
        StartCoroutine(ShowCubesForSelectingAgents());
    }

    private IEnumerator ShowCubesForSelectingAgents()
    {
        //wait for 2 frames, till the grid controller cubes are deactivated
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        GridLightsController.Instance.ShowCubesForSelectingAgents(GameController.Instance.ControlableAgents);
    }


    public override void excuteState()
    {
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
                    LayerMask gridsLayer = 1 << LayerMask.NameToLayer("BlueLightCube");
                    RaycastHit vHit = new RaycastHit();
                    Ray vRay = gameCamera.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(vRay, out vHit, 1000, gridsLayer)) 
                    {
                        Debug.Log("OK " + vHit.collider.gameObject.name);
                        NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInPos(vHit.collider.transform.position.x,vHit.collider.transform.position.z);
                        if (agent == null)
                        {
                            Notification.Instance.ShowNotification("Selected cell is empty");
                            return;
                        }

                        if (!agent.isLocalAgent)
                        {
                            Notification.Instance.ShowNotification("You may only select objects deployed by you!");
                            return;
                        }

                        if (agent.GetComponent<GridObjectBase>().IsFrozen)
                        {
                            Notification.Instance.ShowNotification("Selection is frozen!");
                            return;
                        }


                        if (agent.GetComponent<Tower>())
                        {
                            if (!TowerMoves.Instance.IsTowerMoveAllowed)
                            {
                                Notification.Instance.ShowNotification("Not enough moves!");
                                return;
                            }
                        }

                        //raise the event that a moveable object have been selected
                        GameController.Instance.OnMoveableObjectSelect(agent);
                    }
                }
            }
        }
    }

    public override void endState()
    {
        InstructionsText.Instance.HideInstructionsText();
        //disallow player to end his turn
        GameController.Instance.EndTurnVisiable = false;
        GridLightsController.Instance.DeactivateAllCubes();
    }
}
