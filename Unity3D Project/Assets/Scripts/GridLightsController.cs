using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridLightsController : MonoBehaviour
{
    #region singletonImplementation
    static GridLightsController instance = null;
    public static GridLightsController Instance { get { return instance; } }
    GridLightsController()
    {
        //save time instead of searching for the game controller
        //check if the instance is not null, we are creating more than one instance, warn us
        if (instance != null)
        {
            Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
            Debug.LogError("Original GridLightsController instance is",instance.gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    bool increasingAlpha = false;

    private Material moveLightMaterial;
    /// <summary>
    /// The index of the current blue light cube to be activated.
    /// </summary>
    private int currentMoveIndex = 0;
    [SerializeField]
    private Transform moveLightParent;

    private Material attackLightMaterial;
    /// <summary>
    /// The index of the current red light cube to be activated.
    /// </summary>
    private int currentAttackIndex = 0;
    [SerializeField]
    private Transform attackLightParent;

    // Use this for initialization
    void Awake ()
    {
        //get a pointer to a shared material of each color
        moveLightMaterial = moveLightParent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
        attackLightMaterial = attackLightParent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
        //disable all childs
        DeactivateAllCubes();            
    }

    public void DeactivateAllCubes()
    {
        //reset the current index, so we can used the cubes again from cube 0
        currentMoveIndex = 0;
        currentAttackIndex = 0;
        if (LevelArray.currentLevelArray != null)
        {
            LevelArray.currentLevelArray.releaseAllLightCubes();
        }

        enabled = false;
        foreach(Transform _child in moveLightParent)
        {
            if (_child.gameObject.activeInHierarchy)
            {
                _child.gameObject.SetActive(false);
            }
        }

        foreach(Transform _child in attackLightParent)
        {
            if (_child.gameObject.activeInHierarchy)
            {
                _child.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Shows the cubes for selecting agents in the game.
    /// </summary>
    /// <param name="agents">Agents.</param>
    public void ShowCubesForSelectingAgents(List <NavTileAgent> agents)
    {
        enabled = true;
        foreach(NavTileAgent agent in agents)
        {
            //copy the agent position
            int cellXIndex = LevelArray.currentLevelArray.GetXIndex(agent.transform.position.x);
            int cellZIndex = LevelArray.currentLevelArray.GetZIndex(agent.transform.position.z);
            LevelArray.currentLevelArray.setLightCubeForCell(cellXIndex, cellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
            currentMoveIndex++;
        }
    }

    #region agentMovement
    /// <summary>
    /// Shows the cubes for an agent movement.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void ShowCubesForAgentMovement(NavTileAgent agent)
    {
        enabled = true;
        //activate the deafult post for an agent, this will allow the user to cancel his movement
        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(agent.transform.position.x);
        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(agent.transform.position.z);
        LevelArray.currentLevelArray.setLightCubeForCell(cellXIndex, cellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
        currentMoveIndex++;

        if (agent.movementType == MovementType.PlusShaped || agent.movementType == MovementType.PlusAndXShaped)
        {
            ShowPlusCubesForAgent(agent);
        }

        if (agent.movementType == MovementType.XShaped || agent.movementType == MovementType.PlusAndXShaped)
        {
            ShowXCubesForAgent(agent);
        }
    }

    private void ShowXCubesForAgent(NavTileAgent agent)
    {
        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(agent.transform.position.x);
        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(agent.transform.position.z);

        int newCellXIndex;
        int newCellZIndex;
       
        int steps = agent.AllowedSteps;

        while (steps > 0)
        {
            newCellXIndex = cellXIndex + steps;
            newCellZIndex = cellZIndex + steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            newCellXIndex = cellXIndex + steps;
            newCellZIndex = cellZIndex - steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            newCellXIndex = cellXIndex - steps;
            newCellZIndex = cellZIndex + steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            newCellXIndex = cellXIndex - steps;
            newCellZIndex = cellZIndex - steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            steps--;
        }
    }

    private void ShowPlusCubesForAgent(NavTileAgent agent)
    {
        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(agent.transform.position.x);
        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(agent.transform.position.z);

        int newCellXIndex;
        int newCellZIndex;

        int steps = agent.AllowedSteps;

        while (steps > 0)
        {
            newCellXIndex = cellXIndex + steps;
            newCellZIndex = cellZIndex;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            newCellXIndex = cellXIndex - steps;
            newCellZIndex = cellZIndex;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            newCellXIndex = cellXIndex;
            newCellZIndex = cellZIndex + steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            newCellXIndex = cellXIndex;
            newCellZIndex = cellZIndex - steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                if (!LevelArray.currentLevelArray.IsCubeCellLighted(newCellXIndex, newCellZIndex))
                {
                    LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, moveLightParent.GetChild(currentMoveIndex).gameObject);
                    currentMoveIndex++;
                }
            }

            steps--;
        }
    }
    #endregion
    #region agentAttack
    /// <summary>
    /// Shows the attack cubes for an grid object attack.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void ShowCubesForAgentAttack(GridObjectBase gridObject)
    {
        enabled = true;

        if (gridObject.attackMovementType == MovementType.PlusShaped || gridObject.attackMovementType == MovementType.PlusAndXShaped)
        {
            ShowPlusCubesForObject(gridObject);
        }

        if (gridObject.attackMovementType == MovementType.XShaped || gridObject.attackMovementType == MovementType.PlusAndXShaped)
        {
            ShowXCubesForObject(gridObject);
        }
    }

    private void ShowXCubesForObject(GridObjectBase gridObject)
    {
        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(gridObject.transform.position.x);
        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(gridObject.transform.position.z);

        int newCellXIndex;
        int newCellZIndex;

        int steps = gridObject.attackSteps;

        while (steps > 0)
        {
            newCellXIndex = cellXIndex + steps;
            newCellZIndex = cellZIndex + steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            newCellXIndex = cellXIndex + steps;
            newCellZIndex = cellZIndex - steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            newCellXIndex = cellXIndex - steps;
            newCellZIndex = cellZIndex + steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            newCellXIndex = cellXIndex - steps;
            newCellZIndex = cellZIndex - steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            steps--;
        }
    }

    private void ShowPlusCubesForObject(GridObjectBase gridObject)
    {
        int cellXIndex = LevelArray.currentLevelArray.GetXIndex(gridObject.transform.position.x);
        int cellZIndex = LevelArray.currentLevelArray.GetZIndex(gridObject.transform.position.z);

        int newCellXIndex;
        int newCellZIndex;

        int steps = gridObject.attackSteps;

        while (steps > 0)
        {
            newCellXIndex = cellXIndex + steps;
            newCellZIndex = cellZIndex;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            newCellXIndex = cellXIndex - steps;
            newCellZIndex = cellZIndex;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            newCellXIndex = cellXIndex;
            newCellZIndex = cellZIndex + steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            newCellXIndex = cellXIndex;
            newCellZIndex = cellZIndex - steps;

            if (LevelArray.currentLevelArray.isInIndexRange(newCellXIndex, newCellZIndex))
            {
                NavTileAgent agent = LevelArray.currentLevelArray.GetAgentInCell(newCellXIndex, newCellZIndex);
                if (agent != null)
                {
                    if (agent.isLocalAgent == false)
                    {
                        LevelArray.currentLevelArray.setLightCubeForCell(newCellXIndex, newCellZIndex, attackLightParent.GetChild(currentMoveIndex).gameObject);
                        currentAttackIndex++;
                    }
                }
            }

            steps--;
        }
    }
    #endregion

    // Update is called once per frame
    void Update ()
    {
        //determine whether we want to increase or decrease alpha, or keep the value inchanged
        if (moveLightMaterial.color.a < 0.05f)
        {
            increasingAlpha = true;
        }
        if (moveLightMaterial.color.a > 0.25f)
        {
            increasingAlpha = false;
        }
        //increase/decrease the alpha value based on the above
        if (increasingAlpha)
        {
            SetAlpha( moveLightMaterial.color.a + (0.6f*Time.unscaledDeltaTime) );
        }
        else
        {
            SetAlpha( moveLightMaterial.color.a - (0.6f*Time.unscaledDeltaTime) );
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = moveLightMaterial.color;
        color.a = alpha;
        moveLightMaterial.color = color;

        Color color2 = attackLightMaterial.color;
        color2.a = alpha;
        attackLightMaterial.color = color2;
    }
}
