using UnityEngine;
using System.Collections;

public class StateWaitingForOthersToDeploy : StateBase {
    public override void startState()
    {
        InstructionsText.Instance.ShowInstructionsText("Waiting for other player!");
    }

    public override void excuteState()
    {
    }

    public override void endState()
    {
        InstructionsText.Instance.HideInstructionsText();
    }
}
