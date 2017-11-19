using UnityEngine;
using System.Collections;

public class PhotonController : Photon.PunBehaviour
{
    #region singletonImplementation
    static PhotonController instance = null;
    public static PhotonController Instance
    {
        get {
            if (instance == null)
            {
                Debug.LogError("[ClassName] No instance already created");
            }
            return instance;
        }
    }
    PhotonController()
    {
        //save time instead of searching for the game controller
        //check if the instance is not null, we are creating more than one instance, warn us
        if (instance != null)
        {
            Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
            Debug.LogError("Original PhotonController instance is",instance.gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    bool isRandomChallenge;
    const string PHOTON_VERSION = "1.0";
    string roomName;

    int endDeployCount = 0;

    const int DEPLOY_BATTLE_GO_EVENT_ID = 0;
    const int END_DEPLOY_EVENT = 1;
    const int END_MOVE_EVENT = 2;
    const int MOVE_PIECE_EVENT = 3;
    const int END_GAME_EVENT = 4;

    private void Awake()
    {
        PhotonNetwork.OnEventCall = onEventCallback;
    }
    void OnApplicationQuit() {
        timeOut();
    }

    void connectionFailed()
    {
        Notification.Instance.ShowNotification("Disconnected from server");
        //bring the user back to the main menu
        MainMenu.Instance.ActivatePopUp();
        WaitScreen.Instance.HideWaitScreen();
        RandomChallengeMenu.Instance.DeactivatePopUp();
        ChallengeFriendMenu.Instance.DeactivatePopUp();
    }

    public void timeOut() {
        PhotonNetwork.Disconnect();
    }

    public void playerWon() {
        timeOut();
    }

    public void playerLost() {
        timeOut();
    }

    private void onNewGameStarted()
    {
        endDeployCount = 0;
    }

    public void joinRandomGame() {
        isRandomChallenge = true;
        onNewGameStarted();
        PhotonNetwork.ConnectUsingSettings(PHOTON_VERSION);
    }

    public void createRoomForInvite()
    {
        isRandomChallenge = false;
        onNewGameStarted();
        PhotonNetwork.ConnectUsingSettings(PHOTON_VERSION);
    }

    public override void OnDisconnectedFromPhoton ()
    {
        base.OnDisconnectedFromPhoton ();
        connectionFailed();
    }

    public override void OnConnectedToPhoton ()
    {
        base.OnConnectedToPhoton ();
    }

    public override void OnConnectedToMaster ()
    {
        base.OnConnectedToMaster ();
        PhotonNetwork.JoinLobby();
    }

    public override void OnFailedToConnectToPhoton (DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton (cause);
        connectionFailed();
    }

    public override void OnConnectionFail (DisconnectCause cause)
    {
        base.OnConnectionFail (cause);
        connectionFailed();
    }

    public override void OnJoinedLobby ()
    {
        base.OnJoinedLobby ();
        if (isRandomChallenge)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            RoomOptions options = new RoomOptions();
            options.maxPlayers = 2;
            PhotonNetwork.CreateRoom(null,options,null);
        }
    }

    public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
    {
        base.OnPhotonRandomJoinFailed (codeAndMsg);
        RoomOptions options = new RoomOptions();
        options.maxPlayers = 2;
        PhotonNetwork.CreateRoom(null,options,null);
    }

    public override void OnPhotonJoinRoomFailed (object[] codeAndMsg)
    {
        base.OnPhotonJoinRoomFailed (codeAndMsg);
        connectionFailed();
    }

    public override void OnJoinedRoom ()
    {
        base.OnJoinedRoom ();
        roomName = PhotonNetwork.room.name;
        Debug.Log("Joined room named: " + PhotonNetwork.room.name + ", players count is " + PhotonNetwork.room.playerCount + " , max players is " + PhotonNetwork.room.maxPlayers);
        Notification.Instance.ShowNotification("Connected to server");
        WaitScreen.Instance.HideWaitScreen();
        if (PhotonNetwork.room.playerCount >= 2) {
            GameController.Instance.StartGame();
        }

        if (PhotonNetwork.isMasterClient) {
            CameraController.Instance.SetPlayerOneDefaultPos();
        } else {
            CameraController.Instance.SetPlayerTwoDefaultPos();
        }
    }

    public bool isMaster { get { return PhotonNetwork.isMasterClient; } }

    public override void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps)
    {
        base.OnPhotonPlayerPropertiesChanged (playerAndUpdatedProps);
    }

    public PhotonPlayer getPlayerByID(int pPhotonID) {
        for(int i = 0; i < PhotonNetwork.playerList.Length; i++) {
            if (PhotonNetwork.playerList[i].ID == pPhotonID) {
                return PhotonNetwork.playerList[i];
            }
        }
        return null;
    }

    public void setPlayerProperty(int pIndex, string key, string value) {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add(key, value);
        PhotonNetwork.playerList[pIndex].SetCustomProperties(props);
    }

    public string getPlayerPropery(int pIndex, string key)
    {
        return (string)PhotonNetwork.playerList[pIndex].customProperties[key];
    }

    public void setOtherPlayerProperty(int pIndex, string key, string value)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add(key, value);
        PhotonNetwork.otherPlayers[pIndex].SetCustomProperties(props);
    }

    public string getOtherPlayerPropery(int pIndex, string key)
    {
        return (string)PhotonNetwork.otherPlayers[pIndex].customProperties[key];
    }


    public void setLocalPlayerProperty(string key, string value)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add(key, value);
        PhotonNetwork.player.SetCustomProperties(props);
    }

    public string getLocalPlayerPropery(string key)
    {
        return (string)PhotonNetwork.player.customProperties[key];
    }

    public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected (newPlayer);
        //let's start the game now
        if (PhotonNetwork.room.playerCount >= 2) {
            GameController.Instance.StartGame();
        }
    }

    public override void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected (otherPlayer);
        Debug.Log("OnPhotonPlayerDisconnected");
    }

    public override void OnMasterClientSwitched (PhotonPlayer newMasterClient)
    {
        base.OnMasterClientSwitched (newMasterClient);
        Debug.Log("OnMasterClientSwitched");
    }

    void onEventCallback(byte eventCode, object content, int senderId) {
        int eventID = eventCode;
        string contentString = content as string;
        string[] contentList = contentString.Split(',');
        Debug.Log("onEventCallback:: Event ID is " + eventID);
        switch (eventID)
        {
            case DEPLOY_BATTLE_GO_EVENT_ID:
                const bool IS_LOCAL = false;
                BattleGOs.Instance.DeployBattleGOLocal(contentList[0],int.Parse(contentList[1]),int.Parse(contentList[2]),IS_LOCAL);
                break;
            case END_DEPLOY_EVENT:
                endDeployCount++;
                tryToRaiseEndDeployEvent();
                break;
            case END_MOVE_EVENT:
                GameController.Instance.OnOtherPlayerEndedTurn();
                EventManager.Instance.TriggerEvent(EventManager.EventType.TurnEnded);
                break;
            case MOVE_PIECE_EVENT:
                const bool FROM_LOCAL = false;
                GameController.Instance.MovePieceLocal(int.Parse(contentList[0]),int.Parse(contentList[1]),int.Parse(contentList[2]),int.Parse(contentList[3]),FROM_LOCAL);
                break;
            case END_GAME_EVENT:
                GameController.Instance.OnGameWon();
                break;
            default:
                Debug.LogError("onEventCallback:: Unkowen Event ID is " + eventID);
                break;
        }
    }

    public void MovePieceOnRemote(int fromXIndex, int fromZIndex, int toXIndex, int toZIndex)
    {
        string content = fromXIndex.ToString() + "," + fromZIndex.ToString() + "," + toXIndex.ToString() + "," + toZIndex.ToString();
        PhotonNetwork.RaiseEvent(MOVE_PIECE_EVENT,content,true,null);
    }

    public void DeployBattleGOLocal(string name, int xIndex, int zIndex)
    {
        string content = name + "," + xIndex.ToString() + "," + zIndex.ToString();
        PhotonNetwork.RaiseEvent(DEPLOY_BATTLE_GO_EVENT_ID,content,true,null);
    }

    public void EndGame()
    {
        string content = "";
        PhotonNetwork.RaiseEvent(END_GAME_EVENT,content,true,null);
    }

    public void EndTurn()
    {
        string content = "";
        PhotonNetwork.RaiseEvent(END_MOVE_EVENT,content,true,null);
        EventManager.Instance.TriggerEvent(EventManager.EventType.TurnEnded);
    }

    public void EndDeploying()
    {
        string content = "";
        endDeployCount++;
        PhotonNetwork.RaiseEvent(END_DEPLOY_EVENT,content,true,null);
        tryToRaiseEndDeployEvent();
    }

    private void tryToRaiseEndDeployEvent()
    {
        if (endDeployCount >= 2)
        {
            EventManager.Instance.TriggerEvent(EventManager.EventType.AllPlayersEndedDeploy);
        }
    }
}
