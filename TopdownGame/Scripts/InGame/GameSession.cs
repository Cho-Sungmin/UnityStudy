using System.Threading;

public class GameSession : Session
{
	RoomManager roomManager;
	Room roomInfo;
	UserInfo userInfo;

	public GameSession( ref UserInfo _userInfo ) : base(9933)
	{
		userInfo = _userInfo;
	}

	public GameSession GetInstance( RoomManager mgr )
	{
		roomManager = mgr;

		return this;
	}
	public GameSession GetInstance(  )
	{
		return this;
	}

	public override void Init()
	{
		client.Init();

		//--- Register handler ---//
		client.RegisterHandler( (int) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS , ResponseJoinGame );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_JOIN_GAME_FAIL , ResponseJoinGame );
		

		//--- Request to play game ---//
		//RequestJoinGame();
	}

	public void SetRoomInfo( Room info )
	{
		roomInfo = info;
	}

	public void RequestJoinGame()
	{
		Header header = new Header();

		header.type = (int) PACKET_TYPE.REQ;
		header.func = (int) FUNCTION_CODE.REQ_JOIN_GAME;
		header.len = 0;
		header.sessionID = 0;

		if( roomInfo == null )
			return ;

		//--- Set data with room_id and user_id ---//

		OutputByteStream packet = new OutputByteStream( Header.SIZE + header.len );

		header.Write( packet );

		string room_id = "" + roomInfo.m_id;
		packet.Write( room_id );
		packet.Write( userInfo.m_id );

		client.Send( new InputByteStream( packet ) );
	}

	void ResponseJoinGame( InputByteStream packet )
	{
		Header header = new Header();

		header.Read( packet );

		if( header.func == (int) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS )
		{
			roomManager.LoadRoom( roomInfo );
		}
		//else( packet.head.func == (int) FUNCTION_CODE.RES_JOIN_GAME_FAIL )
		//{
		
		//}
	}

}
