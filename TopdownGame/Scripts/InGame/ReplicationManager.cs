


public class ReplicationManager
{
	GameObjectManager gameObjectManager;

	public ReplicationManager( GameObjectManager mgr )
	{
		gameObjectManager = mgr;
	}

	public void ReplicateCreate( OutputByteStream obstream , GameObjectInfo obj )
	{
		uint objectId = gameObjectManager.GetObjectId( obj );
		uint classId = obj.classId;
		ReplicationHeader header = new ReplicationHeader( ReplicationAction.CREATE , objectId , classId );

		header.Write( obstream );
		obj.Write( obstream );
	}

	public void ReplicateUpdate( OutputByteStream obstream , GameObjectInfo obj )
	{
		uint objectId = gameObjectManager.GetObjectId( obj );
		uint classId = obj.classId;
		ReplicationHeader header = new ReplicationHeader( ReplicationAction.UPDATE , objectId , classId );

		header.Write( obstream );
		obj.Write( obstream );
	}

	public void ReplicateRemove( OutputByteStream obstream , GameObjectInfo obj )
	{
		uint objectId = gameObjectManager.GetObjectId( obj );
		ReplicationHeader header = new ReplicationHeader( ReplicationAction.DESTROY , objectId , 0 );

		header.Write( obstream );
		//obj.Write( obstream );
	}

	public void Replicate( InputByteStream ibstream )
	{
		GameObjectInfo obj;
		ReplicationHeader header = new ReplicationHeader();
		header.Read( ibstream );
		
		ReplicationAction action = header.action;

		if( action == ReplicationAction.CREATE )
		{
			obj = ObjectCreationRegistry.GetInstance().CreateObject( header.classId );
			if( obj == null )
				return ;
			gameObjectManager.AddGameObject( obj , header.objectId );
			obj.Read( ibstream );
			GameObjectInstantiator.InstantiateObject( obj , gameObjectManager.GetObjectId( obj ) );
		}
		else if( action == ReplicationAction.UPDATE )
		{
			obj = gameObjectManager.GetGameObject( header.objectId );
			
			if( obj == null )
			{
				obj = ObjectCreationRegistry.GetInstance().CreateObject( header.classId );
				gameObjectManager.AddGameObject( obj , header.objectId );
				GameObjectInstantiator.InstantiateObject( obj , gameObjectManager.GetObjectId( obj ) ).AddComponent<OtherPlayer>();
				
			}
			obj.Read( ibstream );
		}
		else if( action == ReplicationAction.DESTROY )
		{
			obj = gameObjectManager.GetGameObject( header.objectId );
			gameObjectManager.RemoveGameObject( obj );
		}
		else
		{

		}
	}

}
