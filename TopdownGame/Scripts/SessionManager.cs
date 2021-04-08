using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{

    LoginSession loginSession;
    LobbySession lobbySession;
	GameSession gameSession;

	UserInfo userInfo;


	private void Awake()
	{
		DontDestroyOnLoad(this);
		userInfo = new UserInfo();

		loginSession = new LoginSession( ref userInfo );
        lobbySession = new LobbySession( ref userInfo );	
		gameSession = new GameSession( ref userInfo );
		
	}

	private void Start()
	{
		loginSession.Init();
		lobbySession.Init();
		gameSession.Init();
	}

	private void Update()
	{
		if( loginSession.IsOpen() )
			loginSession.client.DispatchMessages();

		if( lobbySession.IsOpen() )
			lobbySession.client.DispatchMessages();

		if( gameSession.IsOpen() )
			gameSession.client.DispatchMessages();
	}

	public LoginSession GetLoginSession( LoginManager mgr )
	{
		if( loginSession != null )
			return loginSession.GetInstance( mgr );
		else
			return null;
	}

	public LobbySession GetLobbySession( LoadingLobby mgr )
	{
		if( lobbySession != null )
			return lobbySession.GetInstance( mgr );
		else
			return null;
	}
	public LobbySession GetLobbySession( RoomManager mgr )
	{
		if( lobbySession != null )
			return lobbySession.GetInstance( mgr );
		else
			return null;
	}

	public LobbySession GetLobbySession()
	{
		if( lobbySession != null )
			return lobbySession.GetInstance();
		else
			return null;
	}

	public GameSession GetGameSession( RoomManager mgr )
	{
		if( gameSession != null )
			return gameSession.GetInstance( mgr );
		else
			return null;
	}
	public GameSession GetGameSession()
	{
		if( gameSession != null )
			return gameSession.GetInstance();
		else
			return null;
	}
}
