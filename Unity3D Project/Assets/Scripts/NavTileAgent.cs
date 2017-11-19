using UnityEngine;
using System.Collections;

public enum MovementType
{
    XShaped = 0,
    PlusShaped = 1,
    PlusAndXShaped = 2
}

public class NavTileAgent : MonoBehaviour {
    /// <summary>
    /// Whether the Agent is deployed by the localPlayer or remotePlayer/enemy
    /// </summary>
    [HideInInspector]
    public bool isLocalAgent;
    /// <summary>
    /// How many grids is this object allowed to move per turn?
    /// </summary>
    [SerializeField]
    private int allowedSteps;
    [HideInInspector]
    public int AllowedSteps { get { return allowedSteps; } }

    public MovementType movementType;

    public void SetAgentAtPos(Vector3 pos)
    {
        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(pos.x);
        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(pos.z);
        if (LevelArray.currentLevelArray.isInIndexRange(cellXIndex, cellZIndex))
        {
            transform.position = LevelArray.currentLevelArray.GetCellPos(cellXIndex, cellZIndex);
            LevelArray.currentLevelArray.setAsOccupied(transform.position.x,transform.position.z,this);
        }
    }

    public void MoveAgentToPos(int cellXIndex, int cellZIndex)
    {
        if (LevelArray.currentLevelArray.isInIndexRange(cellXIndex, cellZIndex))
        {
            SoundEffectsController.Instance.playSoundEffectOneShot("DeployOnGround");
            LevelArray.currentLevelArray.releaseAsOccupied(transform.position.x,transform.position.z,this);
            transform.position = LevelArray.currentLevelArray.GetCellPos(cellXIndex, cellZIndex);
            LevelArray.currentLevelArray.setAsOccupied(transform.position.x,transform.position.z,this);
        }
    }
}
