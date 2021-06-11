using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingLobby : MonoBehaviour
{
    LobbySession lobbySession;

	private void Awake()
	{
		lobbySession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetLobbySession();
        lobbySession.OpenSession();
	}
	// Start is called before the first frame update
	void Start()
    {
    }
}
