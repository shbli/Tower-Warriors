using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndOfGame : PopUp
{
    #region singletonImplementation
    static EndOfGame instance = null;
    public static EndOfGame Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<EndOfGame>();
            }
            return instance;
        }
    }
    EndOfGame() {
        //save time instead of searching for the game controller
        //check if the instance is not null, we are creating more than one instance, warn us
        if (instance != null) {
            Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
            Debug.LogError("Original EndOfGame instance is",instance.gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    private Text endOfGameText;

    public void OnGameWin()
    {
        endOfGameText.text = "Victory!";
    }
    public void OnGameLost()
    {
        endOfGameText.text = "Lost!";
    }
}
