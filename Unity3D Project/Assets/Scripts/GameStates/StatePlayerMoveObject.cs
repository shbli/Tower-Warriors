using UnityEngine;
using System.Collections;

public class StatePlayerMoveObject : StateBase
{
    Camera gameCamera;
    Vector2 touchStartPos;
    float touchStartTime;
    NavTileAgent selectedAgent;
    private void Awake()
    {
        gameCamera = GetComponent<Camera>();
    }

    public void OnMoveableObjectSelect(NavTileAgent agent)
    {
        selectedAgent = agent;
        GameController.Instance.ChangeGameState(this);
    }

    public override void startState()
    {
        InstructionsText.Instance.ShowInstructionsText("Make a move!");
        StartCoroutine(ShowCubesForAgentMovement());
    }

    private IEnumerator ShowCubesForAgentMovement()
    {
        //wait for 2 frames till all cubes deactivate removed from rendering queue
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        GridLightsController.Instance.ShowCubesForAgentAttack(selectedAgent.GetComponent<GridObjectBase>());
        //wait for 2 frames till all the attack cubes are added to the rendering queue
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        GridLightsController.Instance.ShowCubesForAgentMovement(selectedAgent);
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
                        int currentCellXIndex = LevelArray.currentLevelArray.GetXIndex(selectedAgent.transform.position.x);
                        int currentCellZIndex = LevelArray.currentLevelArray.GetZIndex(selectedAgent.transform.position.z);

                        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(vHit.collider.transform.position.x);
                        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(vHit.collider.transform.position.z);

                        GameController.Instance.MovePiece(currentCellXIndex,currentCellZIndex,cellXIndex,cellZIndex);
                    }
                }
            }
        }
    }

    public override void endState()
    {
        InstructionsText.Instance.HideInstructionsText();
        GridLightsController.Instance.DeactivateAllCubes();
    }
}
