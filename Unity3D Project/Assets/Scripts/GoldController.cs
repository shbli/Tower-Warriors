using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoldController : MonoBehaviour
{
    #region singletonImplementation
    static GoldController instance = null;
    public static GoldController Instance
    {
        get {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GoldController>();
            }
            return instance;
        }
    }
    GoldController()
    {
        //save time instead of searching for the game controller
        //check if the instance is not null, we are creating more than one instance, warn us
        if (instance != null)
        {
            Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
            Debug.LogError("Original GoldController instance is",instance.gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    private Text goldAmountText;

    private int goldAmount;
    public int GoldAmount { get { return goldAmount; } }

    private void Awake()
    {
        goldAmount = 1000;
    }

    public void decreaseGoldBy(int pAmount)
    {
        goldAmount -= pAmount;
        goldAmountText.text = goldAmount.ToString();
    }
}