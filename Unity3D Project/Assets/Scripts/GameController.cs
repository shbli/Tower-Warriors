using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	#region singletonImplementation
	static GameController instance = null;
	public static GameController Instance { get { return instance; } }
	GameController()
	{
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null)
		{
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original GameController instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion

	[SerializeField]
    private GameObject introCamera;
    [SerializeField]
    private GameObject endTurnButton;
    [HideInInspector]
    public bool EndTurnVisiable { set { endTurnButton.SetActive(value); } }

    /// <summary>
    /// The controlable agents list, all locally deployed agents are going to be stored on this list.
    /// </summary>
    private List <NavTileAgent> controlableAgents = new List<NavTileAgent>();
    public List <NavTileAgent> ControlableAgents { get { return controlableAgents; } }

    void Awake()
	{
        #if UNITY_STANDALONE
        Application.runInBackground = true;
        #endif
		BlackLoadingScreen.Instance.gameObject.SetActive(true);
		LoadGameStates();
	}

    public void AddControlableAgent(NavTileAgent agent)
    {
        controlableAgents.Add(agent);
    }

    public void RemoveControlableAgent(NavTileAgent agent)
    {
        controlableAgents.Remove(agent);
        CheckForLoss();
    }

    private void CheckForLoss()
    {
        if (controlableAgents.Count <= 0)
        {
            //the current local player have lost
            TowerMoves.Instance.gameObject.SetActive(false);
            TurnTimer.Instance.gameObject.SetActive(false);
            ChangeGameState(GetComponent<StateUI>());
            EndOfGame.Instance.ActivatePopUp();
            EndOfGame.Instance.OnGameLost();
            PhotonController.Instance.EndGame();
        }
    }

    public void OnGameWon()
    {
        //the current local player have lost
        TowerMoves.Instance.gameObject.SetActive(false);
        TurnTimer.Instance.gameObject.SetActive(false);
        ChangeGameState(GetComponent<StateUI>());
        EndOfGame.Instance.ActivatePopUp();
        EndOfGame.Instance.OnGameWin();
    }

    private void Start()
    {
        EventManager.Instance.StartListening(EventManager.EventType.AllPlayersEndedDeploy,OnAllPlayersEndedDeploy);
    }

    private void ResetGameController()
    {
        controlableAgents.Clear();
    }

	public void StartGame()
	{
        ResetGameController();
		BlackLoadingScreen.Instance.Fadein();
		StartCoroutine(SwitchCameraAfterFade());
	}

	IEnumerator SwitchCameraAfterFade()
	{
		const float CALL_DELAY = 1f;
		yield return new WaitForSeconds(CALL_DELAY);
		//deactivate the popups
		ChallengeFriendMenu.Instance.DeactivatePopUp();
		RandomChallengeMenu.Instance.DeactivatePopUp();
		//activate the camera on here
		GetComponent<Camera>().enabled = true;
		GetComponent<AudioListener>().enabled = true;
        //deactivate main menu camera
		introCamera.gameObject.SetActive(false);
		//show the camera control button
		CameraController.Instance.ShowCameraButton();
		BlackLoadingScreen.Instance.Fadeout();

		//game started, let the player deploy a character or a tower
        TurnTimer.Instance.StartTimer(35);
        ChangeGameState(GetComponent<StateDeployObjectSelection>());
	}

	public void OnDeployableObjectSelected(string deployableObject, int goldAmount)
	{
        GetComponent<StateDeploy>().OnDeployableObjectSelected(deployableObject, goldAmount);
	}

    public void OnMoveableObjectSelect(NavTileAgent agent)
    {
        GetComponent<StatePlayerMoveObject>().OnMoveableObjectSelect(agent);
    }

    public void MovePiece(int fromXIndex, int fromZIndex, int toXIndex, int toZIndex)
    {
        const bool FROM_LOCAL = true;
        bool isMoveSuccess = MovePieceLocal(fromXIndex, fromZIndex, toXIndex, toZIndex, FROM_LOCAL);
        if (isMoveSuccess)
        {
            PhotonController.Instance.MovePieceOnRemote(fromXIndex, fromZIndex, toXIndex, toZIndex);
        }
    }

    /// <summary>
    /// Moves the piece on the local.
    /// </summary>
    /// <returns><c>true</c>, if the piece was moved, <c>false</c> otherwise.</returns>
    /// <param name="fromXIndex">From X index.</param>
    /// <param name="fromZIndex">From Z index.</param>
    /// <param name="toXIndex">To X index.</param>
    /// <param name="toZIndex">To Z index.</param>
    public bool MovePieceLocal(int fromXIndex, int fromZIndex, int toXIndex, int toZIndex, bool pFromLocal)
    {
        NavTileAgent selectedAgent = LevelArray.currentLevelArray.GetAgentInCell(fromXIndex,fromZIndex);
        NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(toXIndex,toZIndex);
        if (agent == null)
        {
            selectedAgent.MoveAgentToPos(toXIndex,toZIndex);
            if (pFromLocal)
            {
                if (selectedAgent.GetComponent<Tower>())
                {
                    TowerMoves.Instance.OneTowerMoved();
                }
                GameController.Instance.OnMoveableObjectMoved();
            }
            return true;
        }
        else
        {
            if (agent.isLocalAgent == pFromLocal)
            {
                if (selectedAgent == agent)
                {
                    if (pFromLocal)
                    {
                        GameController.Instance.OnMoveableObjectMoved();
                    }
                }
                else
                {
                    Notification.Instance.ShowNotification("Prohibited move");
                }
                return false;
            }
            else
            {
                selectedAgent.GetComponent<GridObjectBase>().AttackTarget(agent.GetComponent<GridObjectBase>());
                if (pFromLocal)
                {
                    if (selectedAgent.GetComponent<Tower>())
                    {
                        TowerMoves.Instance.OneTowerMoved();
                    }
                    GameController.Instance.OnMoveableObjectMoved();
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Auto end moves if no move is remaining
    /// </summary>
    public void OnMoveableObjectMoved()
    {
        if (!TowerMoves.Instance.IsTowerMoveAllowed)
        {
            //automatically end the turn
            OnEndTurnClicked();
        }
        else
        {
            ChangeGameState(GetComponent<StateWaitingForMoveToComplete>());
        }
    }

    public void OnEndTurnClicked()
    {
        TowerMoves.Instance.gameObject.SetActive(false);
        TurnTimer.Instance.gameObject.SetActive(false);

        //do nothing if the UI is showing, the game has ended
        if (currentState == GetComponent<StateUI>())
        {
            return;
        }

        //game have not yet started, we are at deploying stage
        if (currentState == GetComponent<StateDeployObjectSelection>() || currentState == GetComponent<StateDeploy>())
        {
            ChangeGameState(GetComponent<StateWaitingForOthersToDeploy>());
            //end deploy
            PhotonController.Instance.EndDeploying();
            //if we ended our turn and deployed 0 characters, we lost the match
            CheckForLoss();
            return;
        }
        else
        {
            ChangeGameState(GetComponent<StateWaitingForOtherTurn>());
            //end turn
            PhotonController.Instance.EndTurn();
            return;
        }
    }

    public void OnOtherPlayerEndedTurn()
    {
        //allow local player to make a move
        TowerMoves.Instance.gameObject.SetActive(true);
        TowerMoves.Instance.ResetRemainingMoves();
        TurnTimer.Instance.StartTimer(20);
        ChangeGameState(GetComponent<StatePlayerSelectObject>());
    }

    /// <summary>
    /// Raises the all players ended deploy event.
    /// </summary>
    private void OnAllPlayersEndedDeploy()
    {
        Debug.Log("All players ended deploy");
        Debug.Log("Rise of turn based gameplay");
        if (PhotonController.Instance.isMaster)
        {
            //make the first move
            TowerMoves.Instance.gameObject.SetActive(true);
            TowerMoves.Instance.ResetRemainingMoves();
            TurnTimer.Instance.StartTimer(20);
            ChangeGameState(GetComponent<StatePlayerSelectObject>());
        }
        else
        {
            //wait for the other player to make a move
            ChangeGameState(GetComponent<StateWaitingForOtherTurn>());
        }   
    }


	#region stateMachine
	private StateBase currentState;
	void LoadGameStates()
	{
		gameObject.AddComponent<StateDeploy>();
        gameObject.AddComponent<StateDeployObjectSelection>();
        gameObject.AddComponent<StateWaitingForOthersToDeploy>();
        gameObject.AddComponent<StatePlayerSelectObject>();
        gameObject.AddComponent<StatePlayerMoveObject>();
        gameObject.AddComponent<StateWaitingForOtherTurn>();
        gameObject.AddComponent<StateWaitingForMoveToComplete>();
		currentState = gameObject.AddComponent<StateUI>();
		//start the initial state
		currentState.startState();
	}

	public void ChangeGameState(StateBase NewState)
	{
		if (currentState != NewState) {
			currentState.endState();
			currentState = NewState;
			currentState.startState();
		}
	}
	#endregion

	private void Update()
	{
		currentState.excuteState();
	}
}
