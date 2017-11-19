using UnityEngine;
using System.Collections;

/// <summary>
/// Waiting for the user to select an object to be deployed
/// </summary>
public class StateDeployObjectSelection : StateBase {
    public override void startState()
    {
        DeployBar.Instance.ShowDeployBar();
        GoldController.Instance.gameObject.SetActive(true);
        //allow player to end his turn at this moment
        GameController.Instance.EndTurnVisiable = true;
    }

    public override void excuteState()
    {
    }

    public override void endState()
    {
        DeployBar.Instance.HideDeployBar();
        GoldController.Instance.gameObject.SetActive(false);
        //disallow player to end his turn
        GameController.Instance.EndTurnVisiable = false;
    }
}
