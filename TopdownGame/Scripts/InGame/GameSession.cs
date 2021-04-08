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
		if( roomInfo == null )
			return ;

		Packet packet = new Packet( TCP.TCP.MAX_PAYLOAD_SIZE );

		packet.head.type = (int) PACKET_TYPE.REQ;
		packet.head.func = (int) FUNCTION_CODE.REQ_JOIN_GAME;

		//--- TODO : Set data with room_id and user_id ---//
		string room_id = "" + roomInfo.m_id;
		packet.SetPayload( System.Text.Encoding.Default.GetBytes(room_id) );
		packet.SetPayload( userInfo.GetId() , 0 , room_id.Length );


		client.Send( packet );
	}

	void ResponseJoinGame( Packet packet )
	{
		if( packet.head.func == (int) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS )
		{
			roomManager.LoadRoom( roomInfo );
		}
		//else( packet.head.func == (int) FUNCTION_CODE.RES_JOIN_GAME_FAIL )
		//{
		
		//}
	}

}
