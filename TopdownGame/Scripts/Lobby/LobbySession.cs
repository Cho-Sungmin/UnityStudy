using System.Collections;
using System.Collections.Generic;
using System;

public class LobbySession : Session
{
	OutputByteStream obstream;
	List<Room> roomList;
	RoomManager roomManager;
	UserInfo userInfo;
	
	public LobbySession( ref UserInfo _userInfo ) : base(1066)
	{	
		roomList = new List<Room>();
		userInfo = _userInfo;
		obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
	}

	public LobbySession GetInstance( RoomManager mgr )
	{
		roomManager = mgr;
		return this;
	}

	public LobbySession GetInstance()
	{
		return this;
	}

	public override void Init()
    {
        client.Init();

        client.RegisterHandler( (int) FUNCTION_CODE.ANY , Heartbeat );
		client.RegisterHandler( (int) FUNCTION_CODE.NOTI_WELCOME , RequestEnterLobby );
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

		userInfo.Write( obstream );

		Header header = new Header();

		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_ENTER_LOBBY;
		header.len = obstream.GetLength();
		header.sessionID = GetSessionID();

		header.InsertFront( ref obstream );
		
		try {
			client.Send( new InputByteStream( obstream ) );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			LOG.printLog( "EXCEPT" , "WARN" , e.Message + " : " + e.TargetSite );
		}

		obstream.Flush();
	}

	public List<Room> GetRoomList()
	{
		return roomList;
	}

    void ResponseEnterLobby( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( ref packet );

		if( header.func == (int) FUNCTION_CODE.RES_ENTER_LOBBY_SUCCESS )
        {
			RequestRoomList( packet );
        }
		else
		{
			LOG.printLog( "REQ" , "NOTI" , "Failed to enter lobby" );
		}
	}

	public void RequestRoomList( InputByteStream packet )
	{
		roomList.Clear();

		Header header = new Header();
		header.Read( ref packet );

		userInfo.Write( obstream );
	
		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_ROOM_LIST;
		header.len = obstream.GetLength();
		header.sessionID = GetSessionID();

		header.InsertFront( ref obstream );

		try {
			client.Send( new InputByteStream( obstream ) );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			LOG.printLog( "EXCEPT" , "WARN" , e.Message + " : " + e.TargetSite );
		}

		obstream.Flush();
	}

	void ResponseRoomList( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( ref packet );

		if( header.func == (int) FUNCTION_CODE.RES_ROOM_LIST_SUCCESS )
        {
			SetRoomList( roomList , packet );
			UnityEngine.SceneManagement.SceneManager.LoadScene( "RoomList" , UnityEngine.SceneManagement.LoadSceneMode.Single );
        }
		else
		{
			LOG.printLog( "REQ" , "NOTI" , "Failed to load room list" );
		}
	}

	public void SetRoomList( List<Room> roomList , InputByteStream ibstream )
	{
		Room room;

		while( ibstream.GetRemainLength() > 0 )
		{
			room = new Room();
			room.Read( ibstream );
			bool isExists = roomList.Exists( (Room element) => 
											{	
												if( room.m_roomId == element.m_roomId )
												{
													element.m_presentMembers = room.m_presentMembers;
													return true;
												}else
													return false;
											});

			if( !isExists )
				roomList.Add(room);
		}

		if( roomManager != null )
			roomManager.SetRoomInfo( roomList );
	}

	public void RequestMakeRoom( InputByteStream roomInfoData )
	{
		Header header = new Header();

		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_MAKE_ROOM;
		header.len = roomInfoData.GetRemainLength();
		header.sessionID = GetSessionID();

		header.Write( ref obstream );
		obstream.Write( roomInfoData.GetBuffer() , header.len );

		try {
			client.Send( new InputByteStream( obstream ) );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			LOG.printLog( "EXCEPT" , "WARN" , e.Message + " : " + e.TargetSite );
		}

		obstream.Flush();
	}

	void ResponseMakeRoom( InputByteStream packet )
	{
		Header header = new Header();

		header.Read( ref packet );
		
		if( header.func == (ushort) FUNCTION_CODE.RES_MAKE_ROOM_SUCCESS )
        {
			//--- Update information in game session  ---//

			SetRoomList( roomList , packet );
			roomManager.JoinRoom( roomList.Count-1 );
        }
		else
		{
			LOG.printLog( "REQ" , "NOTI" , "Failed to make new room" );
		}
	}

	public void RequestEnterLobby( InputByteStream packet )
	{
		NotiWelcomeInfo( packet );
		RequestEnterLobby();
	}

}
