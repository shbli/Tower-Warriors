using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelArray : MonoBehaviour {

	struct GameTile {
		public GameObject cellCube;
		public bool isTile;
        public NavTileAgent occupiedAgent;
        //the light cube that flashes on top of a tile
        public GameObject lightCube;
	}

	[SerializeField]
	GameTile[,] levelMapArray;
	const float STEP_SIZE = 3f;
	int maxZ = int.MinValue;
	int minZ = int.MaxValue;
	int minX = int.MaxValue;
	int maxX = int.MinValue;
	int xSteps = 0;
	int zSteps = 0;
	public static LevelArray currentLevelArray;
	// Use this for initialization
	void Start () {
		currentLevelArray = this;

		foreach(Transform _child in transform) {
			if (_child.position.x < minX) {
				minX = Mathf.RoundToInt(_child.position.x);
			}
			if (_child.position.x > maxX) {
				maxX = Mathf.RoundToInt(_child.position.x);
			}

			if (_child.position.z < minZ) {
				minZ = Mathf.RoundToInt(_child.position.z);
			}
			if (_child.position.z > maxZ) {
				maxZ = Mathf.RoundToInt(_child.position.z);
			}
		}

		xSteps = (int) ((maxX - minX + STEP_SIZE) / STEP_SIZE);
		zSteps = (int) ((maxZ - minZ + STEP_SIZE) / STEP_SIZE);

		levelMapArray = new GameTile[xSteps,zSteps];

		for (int xPos = 0; xPos < xSteps; xPos++) {
			for (int zPos = 0; zPos < zSteps; zPos++) {
				levelMapArray[xPos,zPos].isTile = false;
				levelMapArray[xPos,zPos].occupiedAgent = null;
			}
		}

		foreach(Transform _child in transform) {
			setIsTile(_child.gameObject,_child.position.x,_child.position.z,true);
		}
	}

    public bool isInPosRange(float xPos, float zPos) {
        if (GetXIndex(xPos) >= 0 && GetXIndex(xPos) < xSteps) {
            if (GetZIndex(zPos) >= 0 && GetZIndex(zPos) < zSteps) {
                return true;
            }
        }
        return false;
    }

    public bool isInIndexRange(int xIndex, int zIndex) {
        if (xIndex >= 0 && xIndex < xSteps) {
            if (zIndex >= 0 && zIndex < zSteps) {
                return true;
            }
        }
        return false;
    }

	public Vector3 GetCellPos(int xIndex, int zIndex) {
		return levelMapArray[xIndex,zIndex].cellCube.transform.position;
	}

	public int GetXIndex(float xPos) {
		return Mathf.RoundToInt((xPos - minX)/STEP_SIZE);
	}
	public int GetZIndex(float zPos) {
		return Mathf.RoundToInt((zPos - minZ)/STEP_SIZE);
	}

	public void setIsTile(GameObject cellGO, float xPos, float zPos, bool pIsTile) {
		if (!isInPosRange(xPos,zPos)) {
			Debug.LogError("Index not in range");
			return;
		}

		levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].cellCube = cellGO;
		levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].isTile = pIsTile;
	}

	public bool IsTile(float xPos, float zPos) {
		if (!isInPosRange(xPos,zPos)) {
			return false;
		}

		if (levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].isTile == false) {
			return false;
		}
		return true;
	}

	public bool IsOccupied(float xPos, float zPos) {
		if (!isInPosRange(xPos,zPos)) {
			Debug.LogError("Index not in range");
			return true;
		}

		if (levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent == null) {
			return false;
		}
		return true;
	}

    public NavTileAgent GetAgentInPos(float xPos, float zPos) {
        if (!isInPosRange(xPos,zPos)) {
            Debug.LogError("Index not in range");
            return null;
        }

        return levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent;
    }

    public NavTileAgent GetAgentInCell(int xIndex, int zIndex) {
        if (!isInIndexRange(xIndex,zIndex)) {
            Debug.LogError("Index not in range");
            return null;
        }

        return levelMapArray[xIndex,zIndex].occupiedAgent;
    }

    public void setAsOccupied(float xPos, float zPos, NavTileAgent pAgent) {
        if (!isInPosRange(xPos,zPos)) {
            Debug.LogError("Index not in range");
            return;
        }

        Debug.Log("Cell ID is " + GetXIndex(xPos) + "," + GetZIndex(zPos));

        if (levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent == null) {
            levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent = pAgent;
        }
    }

    public void releaseAsOccupied(float xPos, float zPos, NavTileAgent pAgent) {
        if (!isInPosRange(xPos,zPos)) {
            Debug.LogError("Index not in range");
            return;
        }

        if (levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent == pAgent) {
            levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent = null;
            return;
        } else {
            Debug.LogError("Object occupied by wrong agent, agent = " + levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent,levelMapArray[GetXIndex(xPos),GetZIndex(zPos)].occupiedAgent.gameObject);
        }
    }


    public GameObject GetLightCubeCell(int xIndex, int zIndex) {
        if (!isInIndexRange(xIndex,zIndex)) {
            Debug.LogError("Index not in range");
            return null;
        }

        return levelMapArray[xIndex,zIndex].lightCube;
    }

    /// <summary>
    /// Determines whether this cube cell is lighted.
    /// </summary>
    /// <returns><c>true</c> if this cube cell is lighted; otherwise, <c>false</c>.</returns>
    /// <param name="xIndex">X index.</param>
    /// <param name="zIndex">Z index.</param>
    public bool IsCubeCellLighted(int xIndex, int zIndex) {
        if (!isInIndexRange(xIndex,zIndex)) {
            Debug.LogError("Index not in range");
            return false;
        }

        if (levelMapArray[xIndex,zIndex].lightCube != null)
        {
            if (levelMapArray[xIndex,zIndex].lightCube.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void setLightCubeForCell(int xIndex, int zIndex, GameObject pCube) {
        if (!isInIndexRange(xIndex,zIndex)) {
            Debug.LogError("Index not in range");
            return;
        }

        if (IsCubeCellLighted(xIndex, zIndex))
        {
            Debug.LogError("The cube in this cell have light");
            return;
        }

        levelMapArray[xIndex,zIndex].lightCube = pCube;
        levelMapArray[xIndex,zIndex].lightCube.transform.position = levelMapArray[xIndex,zIndex].cellCube.transform.position;
        levelMapArray[xIndex,zIndex].lightCube.SetActive(true);
    }

    public void releaseAllLightCubes() {
        for (int xPos = 0; xPos < xSteps; xPos++) {
            for (int zPos = 0; zPos < zSteps; zPos++) {
                levelMapArray[xPos,zPos].lightCube = null;
            }
        }
    }

}