using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingLobby : MonoBehaviour
{
    LobbySession lobbySession;

	private void Awake()
	{
		lobbySession = GameObject.Find("SessionManager").GetComponent<SessionManager>().GetLobbySession( this );
        lobbySession.OpenSession();
	}
	// Start is called before the first frame update
	void Start()
    {
        lobbySession.RequestEnterLobby();
    }


    public void LoadRoom()
	{
		SceneManager.LoadScene( "RoomList" , LoadSceneMode.Single );
	}
}
