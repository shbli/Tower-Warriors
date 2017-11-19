using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGOs : MonoBehaviour
{
	#region singletonImplementation
	static BattleGOs instance = null;
	public static BattleGOs Instance
	{
		get {
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<BattleGOs>();
			}
			return instance;
		}
	}
	BattleGOs()
	{
		//save time instead of searching for the game controller
		//check if the instance is not null, we are creating more than one instance, warn us
		if (instance != null)
		{
			Debug.LogError("There's an instance already created, click on the next error to check it", gameObject);
			Debug.LogError("Original BattleGOs instance is",instance.gameObject);
			return;
		}
		instance = this;
	}
	#endregion
	Dictionary <string, GameObject> battleGOsList = new Dictionary<string, GameObject>();
	// Use this for initialization
	void Start ()
	{
		foreach(Transform child in transform)
		{
			battleGOsList.Add(child.name,child.gameObject);
			child.gameObject.SetActive(false);
		}
	}

	public void DeployBattleGO(string name, Vector3 position)
	{
		//deploy locally
        const bool IS_LOCAL = true;
        DeployBattleGOLocal(name,LevelArray.currentLevelArray.GetXIndex(position.x),LevelArray.currentLevelArray.GetZIndex(position.z),IS_LOCAL);
		//deploy on other devices
		PhotonController.Instance.DeployBattleGOLocal(name,LevelArray.currentLevelArray.GetXIndex(position.x),LevelArray.currentLevelArray.GetZIndex(position.z));
	}

	/// <summary>
	/// This methode should only be called by photon events
	/// </summary>
    public void DeployBattleGOLocal(string name, int xIndex, int zIndex, bool isLocal)
	{
        SoundEffectsController.Instance.playSoundEffectOneShot("DeployOnGround");
		Vector3 pos = LevelArray.currentLevelArray.GetCellPos(xIndex,zIndex);
		GameObject battleGO = GameObject.Instantiate(battleGOsList[name],pos,battleGOsList[name].transform.rotation) as GameObject;
		battleGO.SetActive(true);
//		iTween.ScaleFrom(battleGO,
//			iTween.Hash(
//				iTween.HashKeys.time,1f,
//				iTween.HashKeys.scale,Vector3.zero,
//				iTween.HashKeys.easing,iTween.EaseType.easeOutElastic
//			)
//		);
        battleGO.GetComponent<NavTileAgent>().isLocalAgent = isLocal;
        battleGO.GetComponent<NavTileAgent>().SetAgentAtPos(pos);
	}

}
