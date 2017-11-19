using UnityEngine;
using System.Collections;

public class TowerMoves : MonoBehaviour {
    #region singletonImplementation
    static TowerMoves instance = null;
    public static TowerMoves Instance
    {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TowerMoves>();
            }
            return instance;
        }
    }
    TowerMoves()
    {
        //save time instead of searching for the game controller
        //check if the instance is not null, we are creating more than one instance, warn us
        if (instance != null)
        {
            Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
            Debug.LogError("Original TowerMoves instance is",instance.gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    const int MAX_TOWER_MOVES = 1;
    int remainingTowerMoves = 0;
    public bool IsTowerMoveAllowed
    {
        get
        { 
            if (remainingTowerMoves < MAX_TOWER_MOVES)
                return true;
            return false;
        }
    }

    public void ResetRemainingMoves()
    {
        remainingTowerMoves = 0;
        GetComponent<UnityEngine.UI.Text>().text = remainingTowerMoves.ToString();
    }

    public void OneTowerMoved()
    {
        remainingTowerMoves += 1;
        GetComponent<UnityEngine.UI.Text>().text = remainingTowerMoves.ToString();
    }
}
