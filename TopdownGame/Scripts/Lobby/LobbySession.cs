using System.Collections;
using System.Collections.Generic;
using System;

public class LobbySession : Session
{
	List<Room> roomList;
	RoomManager roomManager;

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
		
		OutputByteStream payload = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );

		userInfo.Write( payload );

		OutputByteStream packet = new OutputByteStream( Header.SIZE + payload.GetLength() );

		Header header = new Header();

		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_ENTER_LOBBY;
		header.len = payload.GetLength();
		header.sessionID = GetSessionID();

		header.Write( ref packet );
		packet.Write( payload.GetBuffer() , header.len );

		client.Send( new InputByteStream( packet ) );
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
		Header header = new Header();
		header.Read( ref packet );

		OutputByteStream payload = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		userInfo.Write( payload );
	
		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (UInt16) FUNCTION_CODE.REQ_ROOM_LIST;
		header.len = payload.GetLength();
		header.sessionID = GetSessionID();

		OutputByteStream resPacket = new OutputByteStream( Header.SIZE + header.len );
		header.Write( ref resPacket );
		resPacket.Write( payload.GetBuffer() , header.len );

		

		try {
			client.Send( new InputByteStream(resPacket) );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			LOG.printLog( "EXCEPT" , "WARN" , e.Message + " : " + e.TargetSite );
		}
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

		OutputByteStream packet = new OutputByteStream( Header.SIZE + header.len );

		header.Write( ref packet );
		packet.Write( roomInfoData.GetBuffer() , header.len );

		try {
			client.Send( new InputByteStream( packet ) );
		}
		catch( System.Net.Sockets.SocketException e )
		{
			LOG.printLog( "EXCEPT" , "WARN" , e.Message + " : " + e.TargetSite );
		}
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
