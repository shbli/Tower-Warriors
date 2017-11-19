using UnityEngine;
using System.Collections;

public class DeployBattleGOButton : MonoBehaviour {
    [SerializeField]
    int goldPrice;
	public void OnDeployClicked()
	{
        if (GoldController.Instance.GoldAmount < goldPrice)
        {
            Notification.Instance.ShowNotification("Not enough gold");
            return;
        }

        GameController.Instance.OnDeployableObjectSelected(gameObject.name, goldPrice);
	}
}
