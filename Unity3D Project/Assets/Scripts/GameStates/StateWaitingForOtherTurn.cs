using UnityEngine;
using System.Collections;

public class StateWaitingForOtherTurn : StateBase
{
    public override void startState()
    {
        InstructionsText.Instance.ShowInstructionsText("Waiting for other player turn");
    }

    public override void excuteState()
    {
    }

    public override void endState()
    {
        InstructionsText.Instance.HideInstructionsText();
    }
}
