
public class GameSession : Session
{
	Room roomInfo;
	UserInfo userInfo;

	public ReplicationManager replicationManager;
	public GameObjectManager gameObjectManager;

	public GameSession( ref UserInfo _userInfo ) : base(9933)
	{
		gameObjectManager = new GameObjectManager();
		replicationManager = new ReplicationManager( gameObjectManager );
		userInfo = _userInfo;
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

	public void NotificateReplication( InputByteStream replicationData )
	{
		byte action; replicationData.Read( out action );
		uint objectId; replicationData.Read( out objectId );

		OutputByteStream payload = new OutputByteStream( TCP.TCP.MAX_PAYLOAD_SIZE );
		GameObjectInfo obj = gameObjectManager.GetGameObject( objectId );

		if( action == (byte) ReplicationAction.CREATE )
			replicationManager.ReplicateCreate( payload , obj );
		else if( action == (byte) ReplicationAction.UPDATE )
			replicationManager.ReplicateUpdate( payload , obj );
		else if( action == (byte) ReplicationAction.DESTROY )
			replicationManager.ReplicateRemove( payload , obj );
		else{}

		Header header = new Header();

		header.type = (byte) PACKET_TYPE.NOTI;
		header.func = (ushort) FUNCTION_CODE.NOTI_REPLICATION;
		header.len = payload.GetLength();
		header.sessionID = GetSessionID();

		OutputByteStream reqPacket = new OutputByteStream( Header.SIZE + header.len );

		header.Write( ref reqPacket );
		reqPacket.Write( payload.GetBuffer() , header.len );

		client.Send( new InputByteStream( reqPacket ) );
	}

	public void ProcessReplication( InputByteStream replicationData )
	{
		while( !replicationData.IsEmpty() )
		{
			Header header = new Header(); header.Read( ref replicationData );
			replicationManager.Replicate( replicationData );
		}
	}
}
