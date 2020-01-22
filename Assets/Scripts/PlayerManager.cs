using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get { return _isDead; }
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth;

	[SyncVar]
	private int currentHealth;

	//[SyncVar]
	//private int kills;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	private bool firstSetup = true;

	public void PlayerSetup()
	{
		CmdBroadcastNewPlayerSetup();
	}

	[Command]
	private void CmdBroadcastNewPlayerSetup()
	{
		RpcSetupPlayerOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients()
	{
		if (firstSetup)
		{
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++)
			{
				wasEnabled[i] = disableOnDeath[i].enabled;
			}
		}

		firstSetup = false;

		SetDefaults();
	}

	[ClientRpc]
	public void RpcTakeDamage(int _amount)
	{
		if (isDead)
		{
			return;
		}

		currentHealth -= _amount;

		Debug.Log(transform.name + " health: " + currentHealth);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		isDead = true;

		/*PlayerManager sourcePlayer = GameManager.GetPlayer(_sourceID);
		if (_sourceID != null)
		{
			sourcePlayer.kills++;
		}*/

		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = false;
		}

		//Collider _col = GetComponent<Collider>();
		//if (_col != null)
		//{
		//	_col.enabled = false;
		//}

		Debug.Log(transform.name + " is dead.");

		StartCoroutine(Respawn());
	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		yield return new WaitForSeconds(0.1f);

		SetDefaults();
	}

	public void SetDefaults()
	{
		isDead = false;

		currentHealth = maxHealth;

		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = wasEnabled[i];
		}

		Collider _col = GetComponent<Collider>();
		if (_col != null)
		{
			_col.enabled = true;
		}
	}
}
