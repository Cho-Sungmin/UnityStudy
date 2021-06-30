using System.Collections;
using System.Collections.Generic;
public class GameSession : Session
{
	Room roomInfo;
	UserInfo userInfo;
	OutputByteStream obstream;

	public ReplicationManager replicationManager;
	public GameObjectManager gameObjectManager;

	public GameSession( ref UserInfo _userInfo ) : base(9933)
	{
		gameObjectManager = new GameObjectManager();
		replicationManager = new ReplicationManager( gameObjectManager );
		userInfo = _userInfo;
		obstream = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
	}

	public GameSession GetInstance()
	{
		return this;
	}

	public override void Init()
	{
		client.Init();

		//--- Register handler ---//

		client.RegisterHandler( (int) FUNCTION_CODE.ANY , Heartbeat );
		client.RegisterHandler( (int) FUNCTION_CODE.NOTI_WELCOME , RequestJoinGame );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS , ResponseJoinGame );
		client.RegisterHandler( (int) FUNCTION_CODE.RES_JOIN_GAME_FAIL , ResponseJoinGame );
		client.RegisterHandler( (int) FUNCTION_CODE.NOTI_REPLICATION , NotificateReplication );
		client.RegisterHandler( (int) FUNCTION_CODE.REQ_REPLICATION , ProcessReplication );
		client.RegisterHandler( (int) FUNCTION_CODE.CHAT , Chat );
		
	}

	public void SetRoomInfo( Room info )
	{
		roomInfo = info;
	}

	//--- 서버로 게임 접속 요청 ---//
	public void RequestJoinGame( InputByteStream packet )
	{
		NotiWelcomeInfo( packet );	// 서버로부터 session id 수신 및 갱신

		if( roomInfo == null )
			return ;

		//--- Set data with room_id and user_id ---//
		obstream.Write( roomInfo.m_roomId );
		obstream.Write( userInfo.m_id );

		Header header = new Header();

		header.type = (byte) PACKET_TYPE.REQ;
		header.func = (ushort) FUNCTION_CODE.REQ_JOIN_GAME;
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

	//--- 게임 접속 요청에 대한 응답처리 ---//
	void ResponseJoinGame( InputByteStream packet )
	{
		Header header = new Header();
		header.Read( ref packet );
		ReplicationHeader repHeader = new ReplicationHeader();
		repHeader.Read( packet );


		if( header.func == (ushort) FUNCTION_CODE.RES_JOIN_GAME_SUCCESS )
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("InGame" , UnityEngine.SceneManagement.LoadSceneMode.Single);

			//--- New game object from server ---//
			GameObjectInfo newObject = ObjectCreationRegistry.GetInstance().CreateObject( repHeader.classId );
			newObject.Read( packet );
			gameObjectManager.AddGameObject( newObject , repHeader.objectId );
			gameObjectManager.SetPlayerObject( repHeader.objectId );
		}
	}

	//---  서버로 Replication 정보 전송 ---//
	public void NotificateReplication( InputByteStream replicationData )
	{
		byte action; replicationData.Read( out action );
		uint objectId; replicationData.Read( out objectId );

		GameObjectInfo obj = gameObjectManager.GetGameObject( objectId );

		if( action == (byte) ReplicationAction.CREATE )
			replicationManager.ReplicateCreate( obstream , obj );
		else if( action == (byte) ReplicationAction.UPDATE )
			replicationManager.ReplicateUpdate( obstream , obj );
		else if( action == (byte) ReplicationAction.DESTROY )
			replicationManager.ReplicateRemove( obstream , obj );
		else{}

		Header header = new Header();

		header.type = (byte) PACKET_TYPE.NOTI;
		header.func = (ushort) FUNCTION_CODE.NOTI_REPLICATION;
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

	//--- 서버로부터 받은 Replication 요청 수행 ---//
	public void ProcessReplication( InputByteStream replicationData )
	{
		while( !replicationData.IsEmpty() )
		{
			Header header = new Header(); header.Read( ref replicationData );
			replicationManager.Replicate( replicationData );
		}
	}

	public void Chat( InputByteStream msg )
	{
		Header header = new Header();
		header.Read( ref msg );

		uint targetObjectId;
		string contents;
		msg.Read( out targetObjectId );
		msg.Read( out contents );

		OtherPlayer target = UnityEngine.GameObject.Find( System.Convert.ToString(targetObjectId) ).GetComponent<OtherPlayer>();
		target.DisplayMSG( contents );
	}

	//--- 게임 종료 시, 플레이어 관련 GameObject 삭제요청 및 삭제 ---//
	public void RequestDestroyAllGameObject()
	{
		GameObjectInfo obj = gameObjectManager.GetPlayerObject();
		uint playerObjId = gameObjectManager.GetObjectId( obj );

		ReplicationHeader repHeader = new ReplicationHeader();
		repHeader.action = ReplicationAction.DESTROY;
		repHeader.objectId = playerObjId;
		repHeader.Write( obstream );

		Header header = new Header();
		header.type = (byte) PACKET_TYPE.NOTI;
		header.func = (ushort) FUNCTION_CODE.NOTI_REPLICATION;
		header.len = obstream.GetLength();
		header.sessionID = GetSessionID();
		header.InsertFront( ref obstream );

		client.Send( new InputByteStream( obstream ) );

		obstream.Flush();

		gameObjectManager.DestroyAllGameObject();
	}
}
