using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnTimer : MonoBehaviour {
    #region singletonImplementation
    static TurnTimer instance = null;
    public static TurnTimer Instance
    {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TurnTimer>();
            }
            return instance;
        }
    }
    TurnTimer()
    {
        //save time instead of searching for the game controller
        //check if the instance is not null, we are creating more than one instance, warn us
        if (instance != null)
        {
            Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
            Debug.LogError("Original TurnTimer instance is",instance.gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    Text remainingTimeText;
    float remainingTime;

    public void StartTimer(int pAllowedTime)
    {
        gameObject.SetActive(true);
        remainingTime = (float) pAllowedTime;
        updateTimeText();
    }

	// Update is called once per frame
	void Update ()
    {
        remainingTime -= Time.deltaTime;
        updateTimeText();
        if (remainingTime <= 0)
        {
            gameObject.SetActive(false);
            GameController.Instance.OnEndTurnClicked();
        }
	}


    void updateTimeText()
    {
        int timeAsInt = (int)remainingTime;
        remainingTimeText.text = timeAsInt.ToString() + " seconds";
    }
}
