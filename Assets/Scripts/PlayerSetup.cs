using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour
{
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemoteLayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";

	[SerializeField]
	GameObject playerGraphics;

	[SerializeField]
	GameObject playerUIPrefab;
	private GameObject playerUIInstance;

	private Camera sceneCamera;

	void Start()
	{
		if (!isLocalPlayer)
		{
			DisableComponents();
			AssignRemoteLayer();
		} else
		{
			sceneCamera = Camera.main;
			if (sceneCamera != null)
			{
				sceneCamera.gameObject.SetActive(false);
			}

			SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			GetComponent<PlayerManager>().PlayerSetup();
		}
	}

	void SetLayerRecursively(GameObject obj, int newLayer)
	{
		// I WANT OWN SHADOWS

		/*obj.layer = newLayer;

		foreach (Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, newLayer);
		}*/
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		PlayerManager _player = GetComponent<PlayerManager>();

		GameManager.RegisterPlayer(_netID, _player);
	}

	void RegisterPlayer()
	{
		string ID = "Player " + GetComponent<NetworkIdentity>().netId;
		transform.name = ID;
	}

	void AssignRemoteLayer()
	{
		//gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
		Util.SetLayerRecursively(gameObject, LayerMask.NameToLayer(remoteLayerName));
	}

	void DisableComponents()
	{
		for (int i = 0; i < componentsToDisable.Length; i++)
		{
			componentsToDisable[i].enabled = false;
		}
	}

	void OnDisable()
	{
		Destroy(playerUIInstance);

		if (sceneCamera != null)
		{
			sceneCamera.gameObject.SetActive(true);
		}

		GameManager.UnRegisterPlayer(transform.name);
	}
}
