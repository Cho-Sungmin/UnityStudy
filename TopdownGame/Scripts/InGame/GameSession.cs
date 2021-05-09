
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
		client.RegisterHandler( (int) FUNCTION_CODE.WELCOME , RequestJoinGame );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS , ResponseJoinGame );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_JOIN_GAME_FAIL , ResponseJoinGame );
		
	}

	public void SetRoomInfo( Room info )
	{
		roomInfo = info;
	}

	public void RequestJoinGame( InputByteStream packet )
	{
		NotiWelcomeInfo( packet );

		if( roomInfo == null )
			return ;

		//--- Set data with room_id and user_id ---//
		OutputByteStream payload = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		payload.Write( roomInfo.m_roomId );
		payload.Write( userInfo.m_id );

		Header header = new Header();

		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (ushort) FUNCTION_CODE.REQ_JOIN_GAME;
		header.len = payload.GetLength();
		header.sessionID = GetSessionID();

		OutputByteStream reqPacket = new OutputByteStream( Header.SIZE + header.len );

		header.Write( ref reqPacket );
		reqPacket.Write( payload.GetBuffer() , header.len );

		client.Send( new InputByteStream( reqPacket ) );
	}

	void ResponseJoinGame( InputByteStream packet )
	{
		Header header = new Header();

		header.Read( ref packet );
		roomInfo.Read( packet );

		if( header.func == (ushort) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS )
		{
			roomManager.LoadRoom( roomInfo );
		}
	}

}
