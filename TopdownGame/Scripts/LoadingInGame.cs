using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingInGame : MonoBehaviour
{
	GameSession gameSession;

	private void Awake()
	{
		gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
	}

	private void Start()
	{
		gameSession.RequestJoinGame();
	}
}
