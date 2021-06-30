
using UnityEngine;

public class LoadingInGame : MonoBehaviour
{
	GameSession gameSession;

	private void Awake()
	{
		gameSession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetGameSession();
		gameSession.OpenSession();
	}

	private void Start()
	{
	}
}
