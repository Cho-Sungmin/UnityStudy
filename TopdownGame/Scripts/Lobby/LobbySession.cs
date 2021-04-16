using System.Collections;
using System.Collections.Generic;
using System;

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
		
		OutputByteStream obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		Header header = new Header();

		header.type = (int) PACKET_TYPE.REQ;
		header.func = (int) FUNCTION_CODE.REQ_ENTER_LOBBY;
		header.len = ( UInt32 ) userInfo.GetSize();
		header.sessionID = 0;

		header.Write( obstream );
		userInfo.Write( obstream );


		client.Send( new InputByteStream( obstream ) );
	}

	public List<Room> GetRoomList()
	{
		return roomList;
	}

    public void RequestEnterLobby( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( packet );
	
		header.type = (int) PACKET_TYPE.REQ;
		header.func = (int) FUNCTION_CODE.REQ_ENTER_LOBBY;
		header.len = ( UInt32 ) userInfo.GetSize();
		header.sessionID = 0;

     
		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}
    void ResponseEnterLobby( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( packet );

		if( header.func == (int) FUNCTION_CODE.RES_ENTER_LOBBY_SUCCESS )
        {
			RequestRoomList( packet );
        }
		else
		{
			//Debug.Log("Failed to enter lobby");
		}
	}

	public void RequestRoomList( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( packet );
	
		header.type = (int) PACKET_TYPE.REQ;
		header.func = (int) FUNCTION_CODE.REQ_ROOM_LIST;
		header.len = ( UInt32 ) userInfo.GetSize();
		header.sessionID = 0;


		try {
			client.Send( packet );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}

	void ResponseRoomList( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( packet );

		if( header.func == (int) FUNCTION_CODE.RES_ROOM_LIST_SUCCESS )
        {
			SetRoomList( roomList , packet );
			loadingLobby.LoadRoom();
        }
		else
		{
			//Debug.Log("Failed to load room list");
		}
	}

	public void SetRoomList( List<Room> roomList , InputByteStream ibstream )
	{
		Room room;

		while( !ibstream.IsEmpty() )
		{
			room = new Room();
			room.Read( ibstream );
			roomList.Add(room);
		}

	}

	public void RequestMakeRoom( InputByteStream roomInfoData )
	{
		Header header = new Header();

		header.type = (int) PACKET_TYPE.REQ;
		header.func = (int) FUNCTION_CODE.REQ_MAKE_ROOM;
		header.len = ( UInt32 ) roomInfoData.GetRemainLength();
		header.sessionID = 0;

		OutputByteStream packet = new OutputByteStream( Header.SIZE + header.len );

		header.Write( packet );
		packet.Write( roomInfoData.GetBuffer() , header.len );

		try {
			client.Send( new InputByteStream( packet ) );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			//Debug.LogError( e.Message + " in Function '" + e.TargetSite + "'");
		}
	}

	void ResponseMakeRoom( InputByteStream packet )
	{
		Header header = new Header();

		header.Read( packet );
		
		if( header.func == (int) FUNCTION_CODE.RES_MAKE_ROOM_SUCCESS )
        {
			//--- Update information in game session  ---//

			SetRoomList( roomList , packet );
			roomManager.JoinRoom( roomList.Count-1 );
        }
		else
		{
			//Debug.Log("Failed to load room list");
		}
	}

}
