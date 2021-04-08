using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LobbySession : Session
{
	List<Room> roomList;
	RoomManager roomManager;
	LoadingLobby loadingLobby;

	UserInfo userInfo;
	
	public LobbySession( ref UserInfo _userInfo ) : base(1066)
	{	
		roomList = new List<Room>();
		userInfo = _userInfo;
	}

	public LobbySession GetInstance( RoomManager mgr )
	{
		roomManager = mgr;
		return this;
	}

	public LobbySession GetInstance( LoadingLobby mgr )
	{
		loadingLobby = mgr;
		return this;
	}

	public LobbySession GetInstance()
	{
		return this;
	}

	public override void Init()
    {
        
        client.Init();
        
		client.RegisterHandler( (int) FUNCTION_CODE.REQ_ENTER_LOBBY , RequestEnterLobby );
		client.RegisterHandler( (int) FUNCTION_CODE.REQ_ROOM_LIST , RequestRoomList );
		client.RegisterHandler( (int) FUNCTION_CODE.REQ_MAKE_ROOM , RequestMakeRoom );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_ENTER_LOBBY_SUCCESS , ResponseEnterLobby );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_ENTER_LOBBY_FAIL , ResponseEnterLobby );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_ROOM_LIST_SUCCESS , ResponseRoomList );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_ROOM_LIST_FAIL , ResponseRoomList );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_MAKE_ROOM_SUCCESS , ResponseMakeRoom );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_MAKE_ROOM_FAIL , ResponseMakeRoom );
    }

	
	public void RequestEnterLobby()
	{
		if( userInfo == null )
			return ;
		
		Packet packet = new Packet( TCP.TCP.MAX_PAYLOAD_SIZE );
		
		packet.head.SetHead( (int) PACKET_TYPE.REQ , (int) FUNCTION_CODE.REQ_ENTER_LOBBY , userInfo.Size());
		packet.SetPayload( userInfo.GetBytes() );
		client.Send( packet );
	}

	public List<Room> GetRoomList()
	{
		return roomList;
	}

    public void RequestEnterLobby( Packet packet )
	{
        packet.head.SetHead( (int) PACKET_TYPE.REQ , (int) FUNCTION_CODE.REQ_ENTER_LOBBY , 0);
		//Debug.Log("RequestEnterLobby()");
		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}
    void ResponseEnterLobby( Packet packet )
	{
		//Debug.Log("ResponseEnterLobby()");

		if( packet.head.func == (int) FUNCTION_CODE.RES_ENTER_LOBBY_SUCCESS )
        {
			RequestRoomList( packet );
        }
		else
		{
			//Debug.Log("Failed to enter lobby");
		}
	}

	public void RequestRoomList( Packet packet )
	{
	//Debug.Log("RequestRoomList()");

		packet.head.SetHead( (int) PACKET_TYPE.REQ , (int) FUNCTION_CODE.REQ_ROOM_LIST , 0 );

		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}

	void ResponseRoomList( Packet packet )
	{
		//Debug.Log("ResponseRoomList()");

		JSON.JsonParser jsonParser = new JSON.JsonParser( packet.data );

		if( packet.head.func == (int) FUNCTION_CODE.RES_ROOM_LIST_SUCCESS )
        {
			//--- 받아온 방 목록 데이터를 파싱해서 유니티에 세팅 ---//

			SetRoomList( roomList , jsonParser );

			loadingLobby.LoadRoom();
        }
		else
		{
			//Debug.Log("Failed to load room list");
		}
	}

	public void SetRoomList( List<Room> roomList , JSON.JsonParser jsonParser )
	{
		Room room;
		Hashtable rooms = jsonParser.m_hashtable["room list"] as Hashtable;

		if( rooms == null )
			return ;

		for (int n=0; n<rooms.Count; n++)
		{
			room = new Room();

			room.m_capacity = System.Convert.ToInt32(rooms["capacity"] as string);
			room.m_presentMembers = System.Convert.ToInt32(rooms["presentMembers"] as string);
			room.m_title = rooms["title"] as string;
		
			roomList.Add(room);
		}

	}

	public void RequestMakeRoom( Packet packet )
	{
		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}

	void ResponseMakeRoom( Packet packet )
	{
		
		if( packet.head.func == (int) FUNCTION_CODE.RES_MAKE_ROOM_SUCCESS )
        {
			//--- TODO : Update information in game session  ---//
			JSON.JsonParser jsonParser = new JSON.JsonParser( packet.data );
			SetRoomList( roomList , jsonParser );

			roomManager.JoinRoom( roomList.Count-1 );
        }
		else
		{
			//Debug.Log("Failed to load room list");
		}
	}

}
