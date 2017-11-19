using UnityEngine;
using System.Collections;

public class StateWaitingForMoveToComplete : StateBase
{
    float waitTime;

    public override void startState()
    {
        waitTime = 1f;
    }

    public override void excuteState()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0)
        {
            GameController.Instance.ChangeGameState(GetComponent<StatePlayerSelectObject>());
        }
    }

    public override void endState()
    {
    }
}
